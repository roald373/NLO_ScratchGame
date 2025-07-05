using Microsoft.AspNetCore.SignalR.Client;
using NLO_ScratchGame_Contracts;

public interface IScratchEventPublisher
{
    Task PublishCellScratchedAsync(ScratchResult scratchResult, CancellationToken cancellationToken);
}

public class ScratchEventPublisher : IScratchEventPublisher
{
    private readonly HubConnection _connection;
    private bool _started = false;

    public ScratchEventPublisher()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7237/scratchHub") // URL of your SignalR Hub endpoint
            .Build();
    }

    private async Task StartAsync()
    {
        await _connection.StartAsync();
        _started = true;
    }

    public async Task PublishCellScratchedAsync(ScratchResult scratchResult, CancellationToken cancellationToken)
    {
        if (!_started)
        {
            await StartAsync();
        }


        await _connection.SendAsync("CellScratched", scratchResult, cancellationToken);
    }

    private async Task StopAsync()
    {
        await _connection.StopAsync();
    }
}