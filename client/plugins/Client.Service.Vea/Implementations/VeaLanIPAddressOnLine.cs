using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Client.Service.Vea.Models;
using Common.Libs;
using Common.Libs.Extends;

namespace Client.Service.Vea.Implementations
{
    /// <summary>
    /// 局域网在线设备
    /// </summary>
    public sealed class VeaLanIPAddressOnLine
    {
        public Dictionary<uint, VeaLanIPAddressOnLineItem> Items { get; set; } = new Dictionary<uint, VeaLanIPAddressOnLineItem>();

        public byte[] ToBytes()
        {
            if (Items.Count == 0) return Helper.EmptyArray;

            MemoryStream memoryStream = new MemoryStream();
            byte[] keyBytes = new byte[4];
            foreach (var item in Items.ToArray())
            {
                item.Key.ToBytes(keyBytes);
                memoryStream.Write(keyBytes, 0, keyBytes.Length);

                memoryStream.WriteByte((byte)(item.Value.Online ? 1 : 0));

                ReadOnlySpan<byte> name = item.Value.Name.GetUTF16Bytes();
                memoryStream.WriteByte((byte)name.Length);
                memoryStream.WriteByte((byte)item.Value.Name.Length);
                memoryStream.Write(name);
            }

            return memoryStream.ToArray();
        }

        public void DeBytes(ReadOnlyMemory<byte> memory)
        {
            if (memory.Length == 0) return;

            ReadOnlySpan<byte> span = memory.Span;

            int index = 0;
            while (index < memory.Length)
            {
                uint key = span.Slice(index).ToUInt32();
                index += 4;

                bool online = span[index] == 1;
                index += 1;

                string name = span.Slice(index + 2, span[index]).GetUTF16String(span[index + 1]);
                index += 1 + 1 + span[index];

                Items[key] = new VeaLanIPAddressOnLineItem { Online = online, Name = name };
            }
        }

    }
}