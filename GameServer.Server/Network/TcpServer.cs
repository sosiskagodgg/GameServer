using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Server.Network
{
    public class TcpServer
    {
        private TcpListener? _listener;
        private bool _isRunning;
        private CancellationTokenSource? _acceptCts;

        public bool IsRunning => _isRunning;

        public async Task StartAsync(IPEndPoint endPoint, CancellationToken ct = default)
        {
            if (_isRunning)
                throw new InvalidOperationException("Сервер уже запущен");

            _listener = new TcpListener(endPoint);

            _listener.Start();
            _isRunning = true;
            _acceptCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            _ = Task.Run(() =>  AcceptClientsLoopAsync(_acceptCts.Token)); 
        }

        public async Task StopAsync(CancellationToken ct = default)
        {
            if (!_isRunning) return;

            _acceptCts?.Cancel();
            _listener?.Stop();
            _isRunning = false;
            await Task.CompletedTask;
        }
        private async Task AcceptClientsLoopAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested && _listener != null)
                {
                    var client = await _listener.AcceptTcpClientAsync(ct);

                    _ = Task.Run(() => HadleClientAsync(client, ct));
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
        private async Task HadleClientAsync(TcpClient client,CancellationToken ct)
        {
            try
            {
                using (client)
                {
                    var stream = client.GetStream();
                    var buffer = new byte[4096];

                    while(!ct.IsCancellationRequested && client.Connected)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length,ct);
                        if (bytesRead == 0) break;
                        var reveivedData = new byte[bytesRead];
                        Array.Copy(buffer, reveivedData, bytesRead);

                    }
                        
                }
            }
            catch
            {

            }
        }
    }
}
