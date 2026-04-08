using System.Net.Sockets;
using Microsoft.Extensions.Logging;
namespace GameServer.Server.Network;

public class ClientConnection : IDisposable
{
    private ILogger<ClientConnection> _logger;

    public int Id { get; }
    public bool IsConnected => _client.Connected;

    public event EventHandler<int>? OnConnected;
    public event EventHandler<byte[]?>? DataReceived;
    public event EventHandler<int>? ConnectionClosed;

    private readonly TcpClient _client;
    private bool _disposed;
    private bool _isClosed;

    public ClientConnection(int id, TcpClient tcpClient, ILogger<ClientConnection> logger)
    {
        Id = id;
        _client = tcpClient;
        _logger = logger;
    }

    public async Task StartReadingAsync(CancellationToken ct)
    {
        _logger.LogInformation("Начало работы с клиентом {ClientId}", Id);
        var stream = _client.GetStream();
        var buffer = new byte[4096];
        OnConnected?.Invoke(this, Id);
        try
        {
            while (!ct.IsCancellationRequested && _client.Connected && !_disposed)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                if (bytesRead == 0) break;
                _logger.LogTrace("Получено {BytesRead} байт от клиента {ClientId}", bytesRead, Id);
                var data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead);
                DataReceived?.Invoke(this, data);
            }
        }
        catch (OperationCanceledException)
        {
            
        }
        finally
        {
            if (!_isClosed)
            {
                _isClosed = true;
                ConnectionClosed?.Invoke(this, Id);
            }
        }
    }

    public async Task SendAsync(byte[] data)
    {
        if (!_client.Connected)
            throw new InvalidOperationException("Клиент не подключен");

        var stream = _client.GetStream();
        await stream.WriteAsync(data, 0, data.Length);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _client?.Close();
        _client?.Dispose();

        if (!_isClosed)
            ConnectionClosed?.Invoke(this, Id);

        _disposed = true;
    }
}