using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using NLO_ScratchGame_Contracts;
using NLO_ScratchGame_Database;

namespace NLOScratchGame_Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly IScratchEventPublisher _publisher;

    public Worker(ILogger<Worker> logger, IBus bus, IServiceProvider serviceProvider, IScratchEventPublisher publisher)
    {
        _logger = logger;
        _bus = bus;
        _serviceProvider = serviceProvider;
        _publisher = publisher;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _bus.PubSub.SubscribeAsync<ScratchCommand>("scratch-worker", async command =>
        {
            await ProcessScratchCommand(command, stoppingToken);
        }, stoppingToken);
    }

    internal async Task ProcessScratchCommand(ScratchCommand command, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ScratchGameContext>();

        var userHasAlreadyScratched = await context.ScratchCells.Where(x => x.ScratchedByUserId == command.UserId)
            .AnyAsync(cancellationToken);
        var scratchedCell = await context.ScratchCells
            .Where(c => c.Row == command.Row && c.Column == command.Column)
            .FirstOrDefaultAsync(cancellationToken);

        bool succesfullyScratched = false;
        if (scratchedCell is not null && scratchedCell.ScratchedByUserId is null && !userHasAlreadyScratched)
        {
            scratchedCell.ScratchedByUserId = command.UserId;
            succesfullyScratched = true;
        }

        context.Add(new ScratchAttempt
        {
            UserId = command.UserId,
            Row = command.Row,
            Column = command.Column,
            ScratchedAt = command.ScratchedAt,
            IsSuccessful = succesfullyScratched
        });

        await context.SaveChangesAsync(cancellationToken);

        await _publisher.PublishCellScratchedAsync(new ScratchResult
        {
            UserId = command.UserId,
            Row = command.Row,
            Column = command.Column,
            Prize = !succesfullyScratched ? "?" : scratchedCell?.Prize ?? "",
            SuccessFullyScratched = succesfullyScratched
        }, CancellationToken.None);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _bus.Dispose();
    }
}
