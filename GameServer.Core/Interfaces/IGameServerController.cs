using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Interfaces
{
    public interface IGameServerController
    {
        Task StartAsync(int port, CancellationToken ct = default);

        Task StopAsync(CancellationToken ct = default);

        bool IsRunning { get; }
    }
}
