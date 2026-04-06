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
        Task SendToPlayerAsync(int playerId, SendingData data, CancellationToken ct = default);
        Task SendToRoomAsync(int roomId, SendingData data, CancellationToken ct = default);
        Task BroadcastToAllAsync(SendingData data, CancellationToken ct = default);
    }
}
