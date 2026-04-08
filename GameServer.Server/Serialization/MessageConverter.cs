using GameServer.Server.Serialization;
using System.Text;
using System.Text.Json;
namespace GameServer.Server.Serialization;
public class MessageConverter : IMessageConverter
{
    public T? FromBytes<T>(byte[] data)
    {
        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<T>(json);
    }

    public byte[] ToBytes<T>(T message)
    {
        var json = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(json);
    }
}