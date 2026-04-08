using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        public bool IsRunning => _state.IsRunning;

        public IObservable<DataRecivedEvent> OnDataReceived => throw new NotImplementedException();

        public IObservable<PlayerConnectedEvent> OnPlayerConnected => throw new NotImplementedException();

        public IObservable<PlayerDisconnectedEvent> OnPlayerDisconnected => throw new NotImplementedException();

        public Task BroadcastToAllAsync(byte[] data, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectPlayerAsync(int playerId)
        {
            throw new NotImplementedException();
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

        public Task SendToPlayerAsync(int playerId, byte[] data, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task SendToRoomAsync(int roomId, byte[] data, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(int port, CancellationToken ct = default)
        {
            _lifecycle.StartAsync(port, ct);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken ct = default)
        {
            _lifecycle.StopAsync(ct);
            return Task.CompletedTask;
        }
    }
}
