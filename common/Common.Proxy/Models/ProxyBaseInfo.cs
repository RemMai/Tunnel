using System;
using System.Buffers;
using System.Net;
using Common.Libs.Extends;
using Common.proxy.Enums;
using Common.Server.Models;

namespace Common.Proxy
{
    public class ProxyBaseInfo
    {
        /// <summary>
        /// 保留字段，各协议可以根据自己的实际需求拿去玩儿
        /// </summary>
        public byte Rsv { get; set; }
        /// <summary>
        /// 执行到哪一步了
        /// </summary>
        public EnumProxyStep Step { get; set; } = EnumProxyStep.Command;
        /// <summary>
        /// 连接类型
        /// </summary>
        public EnumProxyCommand Command { get; set; } = EnumProxyCommand.Connect;
        /// <summary>
        /// 地址类型
        /// </summary>
        public EnumProxyAddressType AddressType { get; set; } = EnumProxyAddressType.IPV4;
        /// <summary>
        /// buffer size
        /// </summary>
        public EnumBufferSize BufferSize { get; set; } = EnumBufferSize.KB_8;
        /// <summary>
        /// 插件id 最多 0b1111
        /// </summary>
        public byte PluginId { get; set; }
        /// <summary>
        /// 测试步骤返回 最多 0b1111
        /// </summary>
        public EnumProxyCommandStatusMsg CommandStatusMsg { get; set; }

        public EnumProxyCommandStatus CommandStatus { get; set; }

        /// <summary>
        /// 请求id
        /// </summary>
        public uint RequestId { get; set; }
        /// <summary>
        /// 来源地址，数据从目标端回来的时候回给谁
        /// </summary>
        public IPEndPoint SourceEP { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public Memory<byte> TargetAddress { get; set; }
        public ushort TargetPort { get; set; }

        /// <summary>
        /// 携带的数据
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public Memory<byte> Data { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Memory<byte> Headers { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public int HttpIndex { get; set; }


        public byte[] ToBytes(out int length)
        {
            length = 1 //0000 00 00  rsv + step + command
                     + 1 // 0000 0000 address type + buffer size
                     + 1 //TestResult + PluginId
                     + 1 //CommandResponse
                     + 4  // RequestId
                     + 1  //source length
                     + 1 // target length
                     + Headers.Length + Data.Length;

            int sepLength = 0;
            if (SourceEP != null)
            {
                sepLength = SourceEP.Address.Length();
                length += sepLength + 2;
            }
            if (TargetAddress.Length > 0)
            {
                length += TargetAddress.Length + 2;
            }

            byte[] bytes = ArrayPool<byte>.Shared.Rent(length);
            Memory<byte> memory = bytes.AsMemory(0, length);
            var span = memory.Span;
            int index = 0;


            bytes[index] = (byte)((Rsv << 4) | ((byte)Step) << 2 | (byte)Command);
            index += 1;
            bytes[index] = (byte)(((byte)AddressType << 4) | (byte)BufferSize);
            index += 1;
            bytes[index] = (byte)(((byte)CommandStatusMsg << 4) | PluginId);
            index += 1;

            bytes[index] = (byte)CommandStatus;
            index += 1;

            RequestId.ToBytes(memory.Slice(index));
            index += 4;

            bytes[index] = (byte)sepLength;
            index += 1;
            if (sepLength > 0)
            {
                SourceEP.Address.TryWriteBytes(span.Slice(index), out _);
                index += sepLength;

                ((ushort)SourceEP.Port).ToBytes(memory.Slice(index));
                index += 2;
            }

            bytes[index] = (byte)TargetAddress.Length;
            index += 1;
            if (TargetAddress.Length > 0)
            {
                TargetAddress.CopyTo(memory.Slice(index));
                index += TargetAddress.Length;
                TargetPort.ToBytes(memory.Slice(index));
                index += 2;
            }


            if (Data.Length > 0)
            {
                Memory<byte> target = memory.Slice(index);
                if (Headers.Length > 0)
                {
                    Data.Slice(0, HttpIndex).CopyTo(target);
                    Headers.CopyTo(target.Slice(HttpIndex));
                    Data.Slice(HttpIndex).CopyTo(target.Slice(HttpIndex + Headers.Length));
                }
                else
                {
                    Data.CopyTo(target);
                }
            }
            return bytes;
        }
        public void DeBytes(Memory<byte> bytes)
        {
            var span = bytes.Span;
            int index = 0;

            Rsv = (byte)(span[index] >> 4);
            Step = (EnumProxyStep)((span[index] & 0b0000_1100) >> 2);
            Command = (EnumProxyCommand)(span[index] & 0b0000_0011);
            index++;

            AddressType = (EnumProxyAddressType)(span[index] >> 4);
            BufferSize = (EnumBufferSize)(span[index] & 0b0000_1111);
            index += 1;

            CommandStatusMsg = (EnumProxyCommandStatusMsg)(span[index] >> 4);
            PluginId = (byte)(span[index] & 0b0000_1111);
            index += 1;

            CommandStatus = (EnumProxyCommandStatus)span[index];
            index += 1;

            RequestId = span.Slice(index).ToUInt32();
            index += 4;

            byte epLength = span[index];
            index += 1;
            if (epLength > 0)
            {
                IPAddress ip = new IPAddress(span.Slice(index, epLength));
                index += epLength;
                SourceEP = new IPEndPoint(ip, span.Slice(index, 2).ToUInt16());
                index += 2;
            }

            byte targetepLength = span[index];
            index += 1;
            if (targetepLength > 0)
            {
                TargetAddress = bytes.Slice(index, targetepLength);
                index += targetepLength;
                TargetPort = span.Slice(index, 2).ToUInt16();
                index += 2;
            }

            var data = bytes.Slice(index);
            Data = new byte[data.Length];
            data.CopyTo(Data);
        }
        public static ProxyInfo Debytes(Memory<byte> data)
        {
            ProxyInfo info = new ProxyInfo();
            info.DeBytes(data);
            return info;
        }
        public void Return(byte[] data)
        {
            ArrayPool<byte>.Shared.Return(data);
        }

        public static uint GetRequestId(Memory<byte> bytes)
        {
            return bytes.Span.Slice(3).ToUInt32();
        }
    }
}