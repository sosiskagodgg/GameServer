using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfases;

public interface IServer
{
    Task StartAsync(string address, int port);
    Task StopAsync();
    Task BroadcastAsync(object data);
    Task SendToClientAsync(string clientId, object data);
    IObservable<object> OnDataReceived { get; }
    IReadOnlyList<string> ConnectedClients { get; }
}