using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Events
{
    public class PlayerDisconnectedEvent
    {
        public int PlayerId { get; }
        public PlayerDisconnectedEvent(int playerId) { PlayerId = playerId; }
    }
}
