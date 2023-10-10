using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common.Libs;
using Common.Libs.Extends;
using Common.Server;
using Common.Server.Interfaces;
using Common.Server.Models;

namespace Common.Server.Implementations
{
    public sealed class TcpConnection : Connection
    {
        public TcpConnection(Socket tcpSocket) : base()
        {
            TcpSocket = tcpSocket;

            IPEndPoint address = (TcpSocket.RemoteEndPoint as IPEndPoint) ?? new IPEndPoint(IPAddress.Any, 0);
            if (address.Address.AddressFamily == AddressFamily.InterNetworkV6 && address.Address.IsIPv4MappedToIPv6)
            {
                address = new IPEndPoint(new IPAddress(address.Address.GetAddressBytes()[^4..]), address.Port);
            }
            Address = address;
        }

        /// <summary>
        /// 已连接
        /// </summary>
        public override bool Connected => TcpSocket != null && TcpSocket.Connected;

        /// <summary>
        /// socket
        /// </summary>
        public Socket TcpSocket { get; private set; }
        /// <summary>
        /// 连接类型
        /// </summary>
        public override ServerType ServerType => ServerType.TCP;
        /// <summary>
        /// rtt
        /// </summary>
        public override int RoundTripTime { get; set; }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<bool> Send(ReadOnlyMemory<byte> data, bool unconnectedMessage = false)
        {
            if (Connected)
            {
                try
                {
                    await TcpSocket.SendAsync(data, SocketFlags.None);
                    //SentBytes += (ulong)data.Length;
                    return true;
                }
                catch (Exception ex)
                {
                    Disponse();
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        Logger.Instance.Error(ex);
                }
            }
            return false;
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override async Task<bool> Send(byte[] data, int length, bool unconnectedMessage = false)
        {
            return await Send(data.AsMemory(0, length), unconnectedMessage);
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public override void Disponse()
        {
            if (Relay == false)
            {
                base.Disponse();
                if (TcpSocket != null)
                {
                    TcpSocket.SafeClose();
                    TcpSocket.Dispose();
                }
            }
        }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public override IConnection Clone()
        {
            TcpConnection clone = new TcpConnection(TcpSocket);
            //clone.EncodeEnable(Crypto);
            clone.ReceiveRequestWrap = ReceiveRequestWrap;
            clone.ReceiveResponseWrap = ReceiveResponseWrap;
            return clone;
        }
    }
}