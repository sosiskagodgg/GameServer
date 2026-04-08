using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Models
{
    public class Player
    {
        public int Id { get; }
        public int ConnectionId { get; }
        public Player(int id,int connectionId) {  Id = id;ConnectionId = connectionId; }
    }
}
