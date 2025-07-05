using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using NLO_ScratchGame_Database;
using NLOScratchGame_Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(RabbitHutch.CreateBus("host=localhost;username=guest;password=guest"));
builder.Services.AddDbContext<ScratchGameContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddSingleton<IScratchEventPublisher>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
