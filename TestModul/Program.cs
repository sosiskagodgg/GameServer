using GameServer.Core.Interfaces;
using GameServer.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GameServer.Server.Serialization;
using GameServer.Logic.Game;

var services = new ServiceCollection();



services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Trace);
});

services.AddGameServer();
services.AddSingleton<Game>();


var serviceProvider = services.BuildServiceProvider();



var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Запуск сервера...");
var server = serviceProvider.GetRequiredService<IGameServer>();

await server.StartAsync(8080);
logger.LogInformation("Сервер запущен на порту 8080");



Game? game = serviceProvider.GetService<Game>();

server.OnDataReceived.Subscribe(dataEvent => 
{
    int playerId = dataEvent.PlayerId;
    byte[] data = dataEvent.Data;
    game?.SendData(playerId,data); 
});
