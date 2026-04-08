using System.Net.Sockets;
using System.Text;

namespace GameClient;

public class Program
{
    private static TcpClient? _client;
    private static NetworkStream? _stream;

    public static async Task Main(string[] args)
    {
        _client = new TcpClient();
        await _client.ConnectAsync("127.0.0.1", 8080);
        _stream = _client.GetStream();

        Console.WriteLine("Подключен к серверу");

        // Запускаем приём сообщений
        _ = Task.Run(ReceiveMessages);

        // Отправляем сообщения
        while (true)
        {
            string? message = Console.ReadLine();
            if (message == "/quit") break;

            byte[] data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data);
        }

        _stream.Close();
        _client.Close();
    }

    private static async Task ReceiveMessages()
    {
        var buffer = new byte[4096];

        while (true)
        {
            int bytesRead = await _stream!.ReadAsync(buffer);
            if (bytesRead == 0) break;

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"\nПолучено: {message}\n> ");
        }
    }
}