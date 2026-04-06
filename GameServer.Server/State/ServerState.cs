using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Server.State
{
    public class ServerState : IServerState
    {
        public bool IsRunning { get; set; }
    }
}
