using GameServer.Core.Interfaces;
using GameServer.Core.Models;
using GameServer.DI;
using GameServer.Logic;
using GameServer.Logic.Game;
using GameServer.Logic.Matchmaking;
using GameServer.Server.RoomManagement;
using GameServer.Server.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Numerics;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Trace);
});

services.AddGameServer();
services.AddSingleton<IMatchmakingManager, MatchmakingManager>();

services.AddSingleton<IGameManager, GameManager>(); // ← Добавить эту строку
var serviceProvider = services.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var server = serviceProvider.GetRequiredService<IGameServer>();
var gameManager = serviceProvider.GetRequiredService<IGameManager>();
var matchmaking = serviceProvider.GetRequiredService<IMatchmakingManager>();
var roomManager = serviceProvider.GetRequiredService<IRoomManager>();
server.OnPlayerConnected.Subscribe(connectedEvent =>
{
    // Игрок уже создан в IGameServer? Или нужно создать здесь?
    logger.LogInformation("Игрок {PlayerId} подключился", connectedEvent.Player.Id);
    matchmaking.AddToQueue(connectedEvent.Player);
});

gameManager.GameEnded += (sender, args) =>
{
    logger.LogInformation("Игра в комнате {RoomId} завершена. Игроки возвращаются в очередь.", args.RoomId);

    foreach (var playerId in args.PlayerIds)
    {
        var player = server.GetPlayer(playerId);
        if (player != null)
        {
            matchmaking.AddToQueue(player);
        }
    }
};
// Подписка на входящие сообщения
server.OnDataReceived.Subscribe(async dataEvent =>
{
    string message = System.Text.Encoding.UTF8.GetString(dataEvent.Data);
    var player = server.GetPlayer(dataEvent.PlayerId);

    if (player == null) return;

    if (message == "PLAY")
    {
        matchmaking.AddToQueue(player);
    }
    else if (message == "CANCEL")
    {
        matchmaking.RemoveFromQueue(player.Id);
    }
    else
    {
        // Ход игры (1,2,3)
        gameManager.HandlePlayerInput(player.Id, dataEvent.Data);
    }
});

await server.StartAsync(8080);
logger.LogInformation("Сервер запущен на порту 8080");

Console.WriteLine("Команды для клиента:");
Console.WriteLine("PLAY - встать в очередь");
Console.WriteLine("CANCEL - выйти из очереди");
Console.WriteLine("1,2,3 - ход в игре");

Console.ReadLine();