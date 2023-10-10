using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common.Libs;
using Common.Server.Models;

namespace Common.Server.Interfaces
{
    /// <summary>
    /// 连接对象
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// 连接id
        /// </summary>
        public ulong ConnectId { get; set; }
        /// <summary>
        /// 已连接
        /// </summary>
        public bool Connected { get; }

        #region 中继
        /// <summary>
        /// 是否是中继
        /// </summary>
        public bool Relay { get; set; }
        /// <summary>
        /// 中继对象id，通过谁中继的，就是谁的id，直连的跟连接id一样
        /// </summary>
        public Memory<ulong> RelayId { get; set; }
        /// <summary>
        /// 来源客户端，中继时，数据来源可能不是给你发数据的那个
        /// </summary>
        public IConnection FromConnection { get; set; }
        #endregion

        #region 加密
        /// <summary>
        /// 加密
        /// </summary>
        public bool EncodeEnabled { get; }
        /// <summary>
        /// 加密对象
        /// </summary>
        public ICrypto Crypto { get; }
        /// <summary>
        /// 启用加密
        /// </summary>
        /// <param name="crypto"></param>
        public void EncodeEnable(ICrypto crypto);
        /// <summary>
        /// 移除加密
        /// </summary>
        public void EncodeDisable();
        #endregion

        /// <summary>
        /// 错误
        /// </summary>
        public SocketError SocketError { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public IPEndPoint Address { get; }
        /// <summary>
        /// 连接类型
        /// </summary>
        public ServerType ServerType { get; }
        /// <summary>
        /// rtt
        /// </summary>
        public int RoundTripTime { get; set; }

        #region 接收数据
        /// <summary>
        /// 请求数据包装对象
        /// </summary>
        public MessageRequestWrap ReceiveRequestWrap { get; }
        /// <summary>
        /// 回复数据包装对象
        /// </summary>
        public MessageResponseWrap ReceiveResponseWrap { get; }
        /// <summary>
        /// 接收到的原始数据
        /// </summary>
        public Memory<byte> ReceiveData { get; set; }
        #endregion

        public ulong SentBytes { get; set; }
        /// <summary>
        /// 发送数据权限，当任何一位上为1时，不可发送数据
        /// </summary>
        public byte SendDenied { get; set; }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<bool> Send(ReadOnlyMemory<byte> data, bool unconnectedMessage = false);
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Task<bool> Send(byte[] data, int length, bool unconnectedMessage = false);

        /// <summary>
        /// 销毁
        /// </summary>
        public void Disponse();
        /// <summary>
        /// 克隆，主要用于中继
        /// </summary>
        /// <returns></returns>
        public IConnection Clone();



        #region 回复消息相关

        public Memory<byte> ResponseData { get; }
        public void Write(Memory<byte> data);
        public void Write(ulong num);
        public void Write(ushort num);
        public void Write(ushort[] nums);
        /// <summary>
        /// 英文多用这个
        /// </summary>
        /// <param name="str"></param>
        public void WriteUTF8(string str);
        /// <summary>
        /// 中文多用这个
        /// </summary>
        /// <param name="str"></param>
        public void WriteUTF16(string str);
        /// <summary>
        /// 归还池
        /// </summary>
        public void Return();
        #endregion


        public Task WaitOne();
        public void Release();

        /// <summary>
        /// 引用相等
        /// </summary>
        /// <param name="connection1"></param>
        /// <param name="connection2"></param>
        /// <returns></returns>
        public static bool Equals(IConnection connection1, IConnection connection2)
        {
            if (connection1 == null || connection2 == null)
            {
                return false;
            }
            return ReferenceEquals(connection1, connection2);
        }
        /// <summary>
        /// 引用相等或者地址相等
        /// </summary>
        /// <param name="connection1"></param>
        /// <param name="connection2"></param>
        /// <returns></returns>
        public static bool Equals2(IConnection connection1, IConnection connection2)
        {
            if (connection1 == null || connection2 == null)
            {
                return false;
            }
            return ReferenceEquals(connection1, connection2) || connection1.Address.Equals(connection2.Address);
        }
    }
}
