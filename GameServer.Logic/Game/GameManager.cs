using GameServer.Core.Interfaces;
using GameServer.Core.Models;
using GameServer.Logic.Models;
using GameServer.Server.PlayerManagement;
using GameServer.Server.RoomManagement;
using GameServer.Server.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Logic.Game
{
    public class GameManager : IGameManager
    {
        private readonly ILogger<GameManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRoomManager _roomManager;
        private readonly IPlayerManager _playerManager;
        private readonly Dictionary<int, IGameSession> _games = new(); //roomId=>game
        private readonly Dictionary<int,int> _playerToRoom = new(); //playerId=>roomId

        public GameManager(ILogger<GameManager> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _roomManager = _serviceProvider.GetRequiredService<IRoomManager>();
            _playerManager = _serviceProvider.GetRequiredService<IPlayerManager>();
        }

        public int ActiveGamesCount => _games.Count;

        public event EventHandler<int>? GameCreated;
        public event EventHandler<(int RoomId, List<int> PlayerIds)>? GameEnded;

        public async Task<bool> CreateGame(int[] playerId)
        {
            if (playerId.Length != 2) 
            {
                _logger.LogError("Ошибка создание комнаты, количество игроков - {Lenght}",playerId.Length);
                return false;                
            }

            int roomId = _roomManager.CreateRoom();
            if (_games.ContainsKey(roomId))
            {
                _logger.LogWarning("Игра для этой комнаты уже существует");
                return false;
            }
            Player? firstPlayer = _playerManager.GetPlayer(playerId[0]);

            Player? secondPlayer = _playerManager.GetPlayer(playerId[1]);
            if(firstPlayer == null || secondPlayer == null)
            {
                _logger.LogError("Некорректные айди игроков");
                return false;
            }
            if (IsPlayerInGame(firstPlayer.Id) || IsPlayerInGame(secondPlayer.Id))
            {
                _logger.LogWarning("Один из игроков уже в игре");
                return false;
            }
            _roomManager.JoinRoom(firstPlayer, roomId);
            _roomManager.JoinRoom(secondPlayer, roomId);

            Room? room = _roomManager.GetRoom(roomId);

            if(room == null)
            {
                _logger.LogError("Комната не создалась");
                return false;
            }

            GameSession gameSession = new(room,_serviceProvider.GetRequiredService<IGameServer>(),_serviceProvider.GetRequiredService<IMessageConverter>(),_serviceProvider.GetRequiredService<ILogger<GameSession>>());

            gameSession.GameFinished += (sender, finishedRoomId) => EndGame(finishedRoomId);
            await gameSession.Start();
            _games.Add(roomId, gameSession);
            _playerToRoom.Add(firstPlayer.Id, roomId);
            _playerToRoom.Add(secondPlayer.Id, roomId);


            GameCreated?.Invoke(this, roomId);
            
            return true;

        }

        public void EndGame(int roomId)
        {
            if (!_games.ContainsKey(roomId))
            {
                _logger.LogError("Комнаты с таким айди не существует {Id}", roomId);
                return;
            }

            
            var playersInRoom = _playerToRoom
                .Where(x => x.Value == roomId)
                .Select(x => x.Key)
                .ToList();

            
            _games.Remove(roomId);
            foreach (var p in playersInRoom) _playerToRoom.Remove(p);

            
            GameEnded?.Invoke(this, (roomId, playersInRoom));
        }

        public IReadOnlyList<IGameSession> GetAllActiveGames()
        {
            return _games.Values.Select(x=>(IGameSession)x).ToList().AsReadOnly();
        }

        public IGameSession? GetGameByPlayer(int playerId)
        {
            if (_playerToRoom.TryGetValue(playerId, out int roomId))
            {
                return _games.GetValueOrDefault(roomId);
            }
            return null;
        }

        public IGameSession? GetGameByRoom(int roomId)
        {
            return _games.GetValueOrDefault(roomId);
        }

        public void HandlePlayerDisconnected(int playerId)
        {
            if( _playerToRoom.TryGetValue(playerId,out int roomId))
            {
                EndGame(roomId);
            }
            
        }

        public void HandlePlayerInput(int playerId, byte[] data)
        {
            if (_playerToRoom.TryGetValue(playerId, out int roomId))
            {
                if(_games.TryGetValue(roomId,out IGameSession? gameSession))
                {
                    if (gameSession == null) return;
                    gameSession.HandleInput(playerId, data);
                }
            }
        }

        public bool IsPlayerInGame(int playerId)
        {
            return _playerToRoom.ContainsKey(playerId);
        }
    }
}
