using System.IO;
using System.IO.Pipes;
using System.Xml.Serialization;

namespace TfsDeployer.PowerShellAgent
{
    public class MessagePipe
    {
        private const int BufferSize = 4096;
        private readonly PipeStream _pipeStream;

        public MessagePipe(PipeStream pipeStream)
        {
            _pipeStream = pipeStream;
        }

        public T ReadMessage<T>()
        {
            _pipeStream.ReadMode = PipeTransmissionMode.Message;

            var serializer = new XmlSerializer(typeof(T));
            var buffer = new byte[BufferSize];

            using (var messageStream = new MemoryStream())
            {
                do
                {
                    var bytesRead = _pipeStream.Read(buffer, 0, buffer.Length);
                    messageStream.Write(buffer, 0, bytesRead);
                }
                while (!_pipeStream.IsMessageComplete);

                messageStream.Position = 0;
                return (T)serializer.Deserialize(messageStream);
            }
        }

        public void WriteMessage<T>(T message)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var messageStream = new MemoryStream())
            {
                serializer.Serialize(messageStream, message);
                messageStream.Position = 0;

                var outBuffer = new byte[messageStream.Length];
                messageStream.Read(outBuffer, 0, outBuffer.Length);
                _pipeStream.Write(outBuffer, 0, outBuffer.Length);
            }

        }
    }
}