using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Server.Network
{
    public interface ITcpServer
    {
        Task StartAsync(IPEndPoint endPoint,CancellationToken ct = default);
        Task StopAsync(CancellationToken ct = default);
        bool isRunning {  get; }
    }
}
