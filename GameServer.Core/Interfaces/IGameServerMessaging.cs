using GameServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Interfaces
{
    public interface IGameServerMessaging
    {
        Task SendToPlayerAsync(int playerId, byte[] data, CancellationToken ct = default);
        Task SendToRoomAsync(int roomId, byte[] data, CancellationToken ct = default);
        Task BroadcastToAllAsync(byte[] data, CancellationToken ct = default);
    }
}
