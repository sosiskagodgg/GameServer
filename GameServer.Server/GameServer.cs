using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using GameServer.Core.Events;
using GameServer.Core.Interfaces;
using GameServer.Core.Models;
using GameServer.Server.Lifecycle;
using GameServer.Server.Network;
using GameServer.Server.PlayerManagement;
using GameServer.Server.RoomManagement;
using GameServer.Server.State;
namespace GameServer.Server
{
    public class GameServer : IGameServer
    {
        private readonly ITcpServer _tcpServer;
        private readonly IServerState _state;
        private readonly IServerLifecycle _lifecycle;
        private readonly IRoomManager _roomManager;
        private readonly IPlayerManager _playerManager;
        public GameServer(ITcpServer tcpServer,IServerLifecycle lifecycle, IServerState state,IRoomManager roomManager,IPlayerManager playerManager)
        {
            _lifecycle = lifecycle;
            _tcpServer = tcpServer;
            _state = state;
            _roomManager = roomManager;
            _playerManager = playerManager;

            _tcpServer.OnConnected += (sender, connectionId) => OnClientConnected(connectionId);
            _tcpServer.OnDisconnected += (sender, connectionId) => OnClientDisconnected(connectionId);
            _tcpServer.DataReceived += (sender,data)=> 
            {
                int connectionId = data.ClientId;
                byte[]? receivedData = data.Data;
                DataReceived(connectionId, receivedData);
            };
        }

        public bool IsRunning => _state.IsRunning;

        private readonly Subject<DataRecivedEvent> _dataReceived = new();
        private readonly Subject<PlayerConnectedEvent> _playerConnected = new();
        private readonly Subject<PlayerDisconnectedEvent> _playerDisconnected = new();

        public IObservable<DataRecivedEvent> OnDataReceived => _dataReceived.AsObservable();
        public IObservable<PlayerConnectedEvent> OnPlayerConnected => _playerConnected.AsObservable();
        public IObservable<PlayerDisconnectedEvent> OnPlayerDisconnected => _playerDisconnected.AsObservable();

        private Task OnClientConnected(int connectionId)
        {
            var player = _playerManager.AddPlayer(connectionId);

            _playerConnected.OnNext(new PlayerConnectedEvent(player));

            return Task.CompletedTask;

        }
        private Task OnClientDisconnected(int connectionId)
        {
            var player = _playerManager.GetPlayerByConnectionId(connectionId);
            if (player == null) return Task.CompletedTask;
            
            _roomManager.LeaveRoom(player);

            _playerManager.RemovePlayer(player.Id);

            _playerDisconnected.OnNext(new PlayerDisconnectedEvent(player.Id));
            return Task.CompletedTask;
        }
        private Task DataReceived(int connectionId, byte[] data)
        {
            var player = _playerManager.GetPlayerByConnectionId(connectionId);
            if (player == null) throw new InvalidOperationException("Игрока с таким айди не существует");

            _dataReceived.OnNext(new DataRecivedEvent(player.Id,data));
            return Task.CompletedTask;
        }

        public async Task BroadcastToAllAsync(byte[] data, CancellationToken ct = default)
        {
            await _tcpServer.BroadcastToAllAsync(data);
        }

        public Task DisconnectPlayerAsync(Player player)
        {
            _roomManager.LeaveRoom(player);
            return Task.CompletedTask;
        }

        public Player? GetPlayer(int playerId)
        {
            return _playerManager.GetPlayer(playerId);
        }

        public IReadOnlyList<Player>? GetPlayersInRoom(int roomId)
        {
            return _roomManager.GetPlayersInRoom(roomId);
        }

        public Room? GetRoom(int roomId)
        {
            return _roomManager.GetRoom(roomId);
        }

        public IReadOnlyList<Room>? GetRooms()
        {
            return _roomManager.GetAllRooms();
        }

        public async Task SendToPlayerAsync(int playerId, byte[] data, CancellationToken ct = default)
        {
            int? connectionId = _playerManager.GetPlayer(playerId)?.ConnectionId;
            if (!connectionId.HasValue) 
                throw new InvalidOperationException("Игрока с таким айди не существует");

            await _tcpServer.SendToClientAsync((int)connectionId, data);
        }

        public async Task SendToRoomAsync(int roomId, byte[] data, CancellationToken ct = default)
        {
            List<int> playersId = _roomManager.GetPlayersInRoom(roomId)?.Select(p => p.Id).ToList()?? [];
            
            var tasks = playersId.Select(p=>SendToPlayerAsync(p,data,ct));

            await Task.WhenAll(tasks);
        }

        public async Task StartAsync(int port, CancellationToken ct = default)
        {
            await _lifecycle.StartAsync(port, ct);
        }

        public async Task StopAsync(CancellationToken ct = default)
        {
            await _lifecycle.StopAsync(ct);
        }
    }
}
