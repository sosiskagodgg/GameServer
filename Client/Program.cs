using System.Net.Sockets;
using System.Text;

namespace GameClient;

class Program
{
    static TcpClient? _client;
    static NetworkStream? _stream;

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== Камень-Ножницы-Бумага ===");
        Console.Write("Введите IP сервера (Enter - 127.0.0.1): ");
        string? ip = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(ip)) ip = "127.0.0.1";

        Console.Write("Введите порт (Enter - 8080): ");
        string? portInput = Console.ReadLine();
        int port = string.IsNullOrWhiteSpace(portInput) ? 8080 : int.Parse(portInput);

        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);
            _stream = _client.GetStream();

            Console.WriteLine($"✅ Подключен к {ip}:{port}");
            Console.WriteLine("📝 Команды:");
            Console.WriteLine("   PLAY  - начать поиск игры");
            Console.WriteLine("   CANCEL - выйти из очереди");
            Console.WriteLine("   1 - Камень");
            Console.WriteLine("   2 - Ножницы");
            Console.WriteLine("   3 - Бумага");
            Console.WriteLine("=================================");

            // Запускаем приём сообщений
            _ = Task.Run(ReceiveMessages);

            // Отправка команд
            while (true)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                byte[] data = Encoding.UTF8.GetBytes(input);
                await _stream.WriteAsync(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка: {ex.Message}");
        }
        finally
        {
            _stream?.Close();
            _client?.Close();
        }
    }

    static async Task ReceiveMessages()
    {
        var buffer = new byte[4096];

        try
        {
            while (true)
            {
                int bytesRead = await _stream!.ReadAsync(buffer);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Раскодируем Unicode-эскейп последовательности
                message = System.Text.RegularExpressions.Regex.Unescape(message);

                Console.WriteLine($"\n📨 {message}");
                Console.Write("> ");
            }
        }
        catch
        {
            Console.WriteLine("\n🔌 Соединение разорвано");
        }
    }
}