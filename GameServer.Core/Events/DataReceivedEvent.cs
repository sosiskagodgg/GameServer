using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Events
{
    public class DataRecivedEvent
    {
        public int PlayerId { get; }
        public byte[] Data { get; }
        public DataRecivedEvent(int playerId, byte[] data) { PlayerId = playerId; Data = data; }
    }
}
