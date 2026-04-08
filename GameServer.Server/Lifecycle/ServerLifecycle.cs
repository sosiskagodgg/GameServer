using GameServer.Server.Network;
using GameServer.Server.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Server.Lifecycle
{
    public class ServerLifecycle : IServerLifecycle
    {
        private readonly ITcpServer _tcpServer;
        private readonly IServerState _state;

        public ServerLifecycle(ITcpServer tcpServer, IServerState state)
        {
            _tcpServer = tcpServer;
            _state = state;
        }
        public async Task StartAsync(int port, CancellationToken ct = default)
        {
            var endPoint = new IPEndPoint(IPAddress.Any, port);

            await _tcpServer.StartAsync(endPoint, ct);

            _state.IsRunning = true;
        }
        public async Task StopAsync(CancellationToken ct = default)
        {
            await _tcpServer.StopAsync(ct);

            _state.IsRunning = false;
        }
        public bool IsRunning => _state.IsRunning;
    }
}
