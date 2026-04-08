using GameServer.Core.Events;
using GameServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Interfaces
{
    public interface IGameServerMonitor
    {
        IReadOnlyList<Player>? GetPlayersInRoom(int roomId);
        Player? GetPlayer(int playerId);
        IReadOnlyList<Room>? GetRooms();
        Room? GetRoom(int roomId);
        Task DisconnectPlayerAsync(int playerId);

        IObservable<DataRecivedEvent> OnDataReceived { get; }
        IObservable<PlayerConnectedEvent> OnPlayerConnected { get; }
        IObservable<PlayerDisconnectedEvent> OnPlayerDisconnected { get; }
    }
}
