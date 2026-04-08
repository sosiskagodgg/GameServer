using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Server.Network
{
    public class TcpServer : ITcpServer
    {

        private ILogger<TcpServer> _logger;
        private ILogger<ClientConnection> _clientLogger;
        public TcpServer(ILogger<TcpServer> logger,ILogger<ClientConnection> clientLogger)
        {
            _logger = logger;
            _clientLogger = clientLogger;
        }

        private TcpListener? _listener;
        private bool _isRunning;
        private CancellationTokenSource? _acceptCts;
        

        private List<ClientConnection> _connections { get; set; } = new();
        private readonly object _connectionsLock = new();

        private int _nextId = 1;
        public bool isRunning => _isRunning;


        public event EventHandler<(int ClientId, byte[]? Data)>? DataReceived;
        public event EventHandler<int>? OnConnected;
        public event EventHandler<int>? OnDisconnected;
        public Task StartAsync(IPEndPoint endPoint, CancellationToken ct = default)
        {
            if (_isRunning)
            {
                _logger.LogError("Сервер уже запущен");
                return Task.CompletedTask;
            }
            

            _listener = new TcpListener(endPoint);

            _listener.Start();
            _isRunning = true;
            _acceptCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            _ = Task.Run(() =>  AcceptClientsLoopAsync(_acceptCts.Token));

            return Task.CompletedTask;
        }
        public async Task StopAsync(CancellationToken ct = default)
        {
            if (!_isRunning) return;

            _acceptCts?.Cancel();
            _listener?.Stop();
            _isRunning = false;
            await Task.CompletedTask;
        }

        public async Task SendToClientAsync(int clientId,byte[] data)
        {
            ClientConnection? clientConnection;
            lock (_connectionsLock) { clientConnection = _connections.FirstOrDefault(c => c.Id == clientId); }
            if (clientConnection == null) throw new InvalidOperationException("Клиента с таким Id не существует");
            await clientConnection.SendAsync(data);
        }

        private async Task AcceptClientsLoopAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested && _listener != null)
                {
                    var client = await _listener.AcceptTcpClientAsync(ct);

                    ClientConnection clientConnection = new ClientConnection(_nextId, client, _clientLogger);
                    lock (_connectionsLock)
                    {
                        _connections.Add(clientConnection);
                        _nextId++;
                    }
                    

                    clientConnection.DataReceived += (_, data) => { DataReceived?.Invoke(this, (clientConnection.Id,data)); };
                    clientConnection.ConnectionClosed += (_, id) => { OnDisconnected?.Invoke(this, clientConnection.Id); lock (_connectionsLock) { _connections.RemoveAll(c => c.Id == id); } };
                    clientConnection.OnConnected += (_, id) => { OnConnected?.Invoke(this, id); };

                    _ = Task.Run(() => clientConnection.StartReadingAsync(ct));
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task BroadcastToAllAsync(byte[] data)
        {
            List<ClientConnection> connections;
            lock (_connectionsLock)
            {
                connections = _connections.ToList();
            }
            var tasks = connections.Select(p=>p.SendAsync(data));
            await Task.WhenAll(tasks);
        }
    }
}
