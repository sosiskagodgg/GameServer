using System.Text;
using System.Text.Json;

public static class MessageConverter
{
    public static T? FromBytes<T>(byte[] data)
    {
        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<T>(json);
    }

    public static byte[] ToBytes<T>(T message)
    {
        var json = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(json);
    }
}