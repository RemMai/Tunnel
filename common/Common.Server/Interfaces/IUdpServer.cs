using System;
using System.Net;
using System.Threading.Tasks;
using LiteNetLib;

namespace Common.Server.Interfaces
{
    public interface IUdpServer : IServer
    {
        public void Start(int port, int timeout = 20000);
        public Task<IConnection> CreateConnection(IPEndPoint address);
        public NetPeer Connect(IPEndPoint address);

        public bool SendUnconnectedMessage(Memory<byte> message, IPEndPoint address);

    }
}
