using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common.Libs;
using Common.Server;
using Common.Server.Interfaces;
using Common.Server.Models;
using LiteNetLib;

namespace Common.Server.Implementations
{
    public sealed class RudpConnection : Connection
    {
        public RudpConnection(NetPeer peer, IPEndPoint address) : base()
        {
            NetPeer = peer;

            if (address.Address.AddressFamily == AddressFamily.InterNetworkV6 && address.Address.IsIPv4MappedToIPv6)
            {
                address = new IPEndPoint(new IPAddress(address.Address.GetAddressBytes()[^4..]), address.Port);
            }

            Address = address;
        }

        /// <summary>
        /// 已连接
        /// </summary>
        public override bool Connected => NetPeer != null && NetPeer.ConnectionState == ConnectionState.Connected;
        /// <summary>
        /// 连接对象
        /// </summary>
        public NetPeer NetPeer { get; private set; }
        /// <summary>
        /// 连接类型
        /// </summary>
        public override ServerType ServerType => ServerType.UDP;
        /// <summary>
        /// rtt
        /// </summary>
        public override int RoundTripTime { get; set; }


        public static TokenBucketRatelimit tokenBucketRatelimit = new TokenBucketRatelimit(50 * 1024);
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
                    if (unconnectedMessage == false)
                    {
                        int index = 0;
                        while (NetPeer.GetPacketsCountInReliableQueue(0, true) > 75)
                        {
                            if (index >= 10000 / 30 || Connected == false)
                            {
                                return false;
                            }
                            NetPeer.Update();
                            await Task.Delay(30);
                            index++;
                        }
                        int len = 0;
                        do
                        {
                            len = tokenBucketRatelimit.Try(data.Length);
                            if (len < data.Length)
                            {
                                await Task.Delay(30);
                            }
                            if (Connected == false) return false;
                        } while (len < data.Length);

                        NetPeer.Send(data, 0, data.Length, DeliveryMethod.ReliableOrdered);
                        //SentBytes += (ulong)data.Length;
                        NetPeer.Update();
                    }
                    else
                    {
                        NetPeer.NetManager.SendUnconnectedMessage(data, 0, data.Length, Address);
                    }


                    return true;
                }
                catch (Exception ex)
                {
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        Logger.Instance.Error(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void Disponse()
        {
            if (Relay == false)
            {
                base.Disponse();
                if (NetPeer != null)
                {
                    if (NetPeer.ConnectionState == ConnectionState.Connected)
                    {
                        NetPeer.Disconnect();
                    }
                    NetPeer = null;
                }
            }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public override IConnection Clone()
        {
            RudpConnection clone = new RudpConnection(NetPeer, Address);
            //clone.EncodeEnable(Crypto);
            clone.ReceiveRequestWrap = ReceiveRequestWrap;
            clone.ReceiveResponseWrap = ReceiveResponseWrap;
            return clone;
        }
    }

}