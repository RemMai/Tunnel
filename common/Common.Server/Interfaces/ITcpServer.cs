using System.Net.Sockets;

namespace Common.Server.Interfaces
{
    public interface ITcpServer : IServer
    {
        public void SetBufferSize(int bufferSize = 8 * 1024);
        public IConnection BindReceive(Socket socket, int bufferSize = 8 * 1024);
        public IConnection CreateConnection(Socket socket);
    }
}
