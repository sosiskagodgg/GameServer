using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Events
{
    public class PlayerConnectedEvent
    {
        public int PlayerId { get; }
        public int ConnectionId { get; }
        public PlayerConnectedEvent(int playerId,int connectionId) { PlayerId = playerId;ConnectionId = connectionId; }
    }
}
