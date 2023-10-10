using System;
using Common.Libs.Extends;

namespace Client.Service.Vea.Models
{
    /// <summary>
    /// ip更新消息模型
    /// </summary>
    public sealed class IPAddressInfo
    {
        /// <summary>
        /// ip 小端
        /// </summary>
        public uint IP { get; set; }
        /// <summary>
        /// 局域网网段
        /// </summary>
        public VeaLanIPAddress[] LanIPs { get; set; }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            var bytes = new byte[
                1   //ip length
                + 4 //ip
                + 1 // LanIPs length
                + LanIPs.Length * 17];
            var span = bytes.AsSpan();

            int index = 0;
            bytes[index] = 4;
            index += 1;
            IP.ToBytes(bytes.AsMemory(index));
            index += 4;

            bytes[index] = (byte)LanIPs.Length;
            index += 1;
            for (int i = 0; i < LanIPs.Length; i++)
            {
                LanIPs[i].IPAddress.ToBytes(bytes.AsMemory(index));
                index += 4;
                bytes[index] = LanIPs[i].MaskLength;
                index += 1;
                LanIPs[i].MaskValue.ToBytes(bytes.AsMemory(index));
                index += 4;
                LanIPs[i].NetWork.ToBytes(bytes.AsMemory(index));
                index += 4;
                LanIPs[i].Broadcast.ToBytes(bytes.AsMemory(index));
                index += 4;
            }

            return bytes;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="memory"></param>
        public void DeBytes(ReadOnlyMemory<byte> memory)
        {
            var span = memory.Span;

            int index = 0;

            byte ipLength = span[index];
            index += 1;

            IP = span.Slice(index, ipLength).ToUInt32();
            index += ipLength;

            byte lanipLength = span[index];
            index += 1;

            LanIPs = new VeaLanIPAddress[lanipLength];
            for (int i = 0; i < lanipLength; i++)
            {
                ReadOnlyMemory<byte> ip = memory.Slice(index, 4);
                index += 4;
                byte mask = span[index];
                index += 1;

                ReadOnlyMemory<byte> maskvalue = memory.Slice(index, 4);
                index += 4;

                ReadOnlyMemory<byte> network = memory.Slice(index, 4);
                index += 4;

                ReadOnlyMemory<byte> broadcast = memory.Slice(index, 4);
                index += 4;

                LanIPs[i] = new VeaLanIPAddress
                {
                    IPAddress = ip.ToUInt32(),
                    MaskLength = mask,
                    MaskValue = maskvalue.ToUInt32(),
                    NetWork = network.ToUInt32(),
                    Broadcast = broadcast.ToUInt32(),
                };
            }
        }
    }
}