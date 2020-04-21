using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using TestDll;

namespace Common
{
    public class SerializationManager
    {
        BinaryFormatter s_bf = new BinaryFormatter();

        /// <summary>
        /// 反序列化
        /// </summary>
        public Message03 DeserializeClass(TcpClient _client)
        {
            NetworkStream clientStream = _client.GetStream();
            byte[] buffer = new byte[_client.Available];
            clientStream.Read(buffer,0,buffer.Length);

            MemoryStream ms = new MemoryStream(buffer);
            Message03 result = (Message03)s_bf.Deserialize(ms);

            return result;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public void SerializeClass(TcpClient _client, Message03 _sendmsg)
        {
            MemoryStream ms = new MemoryStream();
            s_bf.Serialize(ms,_sendmsg);
            byte[] buffer = ms.ToArray();
            _client.GetStream().Write(buffer, 0, buffer.Length);
        }

    }
}
