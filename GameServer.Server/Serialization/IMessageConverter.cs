using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Server.Serialization
{
    public interface IMessageConverter
    {
        T? FromBytes<T>(byte[] data);
        byte[] ToBytes<T>(T message);
    }
}
