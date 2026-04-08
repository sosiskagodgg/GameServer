using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Models;
namespace GameServer.Core.Events
{
    public class PlayerConnectedEvent
    {
        public Player Player { get; }
        public PlayerConnectedEvent(Player player) {  Player = player; }
    }
}
