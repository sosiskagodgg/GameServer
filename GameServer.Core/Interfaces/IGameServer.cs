using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Events;
using GameServer.Core.Models;
namespace GameServer.Core.Interfaces
{
    public interface IGameServer : IGameServerController,IGameServerMessaging,IGameServerMonitor
    {
    }
}
