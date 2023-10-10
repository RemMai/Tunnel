using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Libs;
using Common.Libs.Extends;
using Common.Server;
using Common.Server.Interfaces;
using Common.Server.Models;
using Serilog;

namespace Common.Server.Implementations
{
    public abstract class Connection : IConnection
    {
        public Connection()
        {
        }

        private ulong connectId = 0;
        /// <summary>
        /// 连接id
        /// </summary>
        public ulong ConnectId
        {
            get
            {
                return connectId;
            }
            set
            {
                connectId = value;
            }
        }

        /// <summary>
        /// 已连接
        /// </summary>
        public virtual bool Connected => false;
        /// <summary>
        /// 错误
        /// </summary>
        public SocketError SocketError { get; set; } = SocketError.Success;
        /// <summary>
        /// 地址
        /// </summary>
        public IPEndPoint Address { get; protected set; }
        /// <summary>
        /// 连接类型
        /// </summary>
        public virtual ServerType ServerType => ServerType.UDP;
        /// <summary>
        /// rtt
        /// </summary>
        public virtual int RoundTripTime { get; set; }


        #region 中继
        /// <summary>
        /// 已中继
        /// </summary>
        public bool Relay { get; set; } = false;
        /// <summary>
        /// 中继路线
        /// </summary>
        public Memory<ulong> RelayId { get; set; }
        /// <summary>
        /// 来源连接
        /// </summary>
        public IConnection FromConnection { get; set; }
        #endregion

        #region 加密
        /// <summary>
        /// 启用加密
        /// </summary>
        public bool EncodeEnabled => Crypto != null;
        /// <summary>
        /// 加密类
        /// </summary>
        public ICrypto Crypto { get; private set; }
        /// <summary>
        /// 启用加密
        /// </summary>
        /// <param name="crypto"></param>
        public void EncodeEnable(ICrypto crypto)
        {
            Crypto = crypto;
        }
        /// <summary>
        /// 移除加密
        /// </summary>
        public void EncodeDisable()
        {
            Crypto = null;
        }
        #endregion

        #region 接收数据
        /// <summary>
        /// 接收请求数据
        /// </summary>
        public MessageRequestWrap ReceiveRequestWrap { get; set; }
        /// <summary>
        /// 接收回执数据
        /// </summary>
        public MessageResponseWrap ReceiveResponseWrap { get; set; }
        /// <summary>
        /// 接收数据
        /// </summary>
        public Memory<byte> ReceiveData { get; set; }
        #endregion

        #region 回复数据
        public Memory<byte> ResponseData { get; private set; }
        private byte[] responseData;
        private int length = 0;

        public void Write(Memory<byte> data)
        {
            ResponseData = data;
        }
        public void Write(ulong num)
        {
            length = 8;
            responseData = ArrayPool<byte>.Shared.Rent(length);
            num.ToBytes(responseData);
            ResponseData = responseData.AsMemory(0, length);
        }
        public void Write(ushort num)
        {
            length = 2;
            responseData = ArrayPool<byte>.Shared.Rent(length);
            num.ToBytes(responseData);
            ResponseData = responseData.AsMemory(0, length);
        }
        public void Write(ushort[] nums)
        {
            length = nums.Length * 2;
            responseData = ArrayPool<byte>.Shared.Rent(length);
            nums.ToBytes(responseData);
            ResponseData = responseData.AsMemory(0, length);
        }
        /// <summary>
        /// 英文多用这个
        /// </summary>
        /// <param name="str"></param>
        public void WriteUTF8(string str)
        {
            var span = str.AsSpan();
            responseData = ArrayPool<byte>.Shared.Rent((span.Length + 1) * 3 + 8);
            var memory = responseData.AsMemory();

            int utf8Length = span.ToUTF8Bytes(memory.Slice(8));
            span.Length.ToBytes(memory);
            utf8Length.ToBytes(memory.Slice(4));
            length = utf8Length + 8;

            ResponseData = responseData.AsMemory(0, length);
        }
        /// <summary>
        /// 中文多用这个
        /// </summary>
        /// <param name="str"></param>
        public void WriteUTF16(string str)
        {
            var span = str.GetUTF16Bytes();
            length = span.Length + 4;
            responseData = ArrayPool<byte>.Shared.Rent(length);
            str.Length.ToBytes(responseData);
            span.CopyTo(responseData.AsSpan(4));

            ResponseData = responseData.AsMemory(0, length);
        }
        /// <summary>
        /// 归还池
        /// </summary>
        public void Return()
        {
            if (length > 0 && ResponseData.Length > 0)
            {
                ArrayPool<byte>.Shared.Return(responseData);
            }
            ResponseData = Helper.EmptyArray;
            responseData = null;
            length = 0;
        }
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
        public abstract Task<bool> Send(ReadOnlyMemory<byte> data, bool logger = false);
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public abstract Task<bool> Send(byte[] data, int length, bool logger = false);

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Disponse()
        {
            try
            {
                if (Semaphore != null)
                {
                    if (locked)
                    {
                        locked = false;
                        Semaphore.Release();
                    }
                    Semaphore.Dispose();
                }
                Semaphore = null;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
            //ReceiveRequestWrap = null;
            //ReceiveResponseWrap = null;
        }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public abstract IConnection Clone();


        SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        bool locked = false;
        public virtual async Task WaitOne()
        {
            try
            {
                if (Semaphore != null)
                {
                    locked = true;
                    await Semaphore.WaitAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }

        }
        public virtual void Release()
        {
            try
            {
                if (Semaphore != null)
                {
                    locked = false;
                    Semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
    }

}