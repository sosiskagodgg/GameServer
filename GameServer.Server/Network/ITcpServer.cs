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
        Task SendToClientAsync(int clientId, byte[] data);
        Task BroadcastToAllAsync(byte[] data);

        event EventHandler<(int ClientId, byte[]? Data)>? DataReceived;
        event EventHandler<int>? OnConnected;
        event EventHandler<int>? OnDisconnected;
        bool isRunning {  get; }
    }
}
