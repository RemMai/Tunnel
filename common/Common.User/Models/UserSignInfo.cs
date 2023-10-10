using System;
using Common.Libs.Extends;

namespace Common.User.Models
{
    public sealed class UserSignInfo
    {
        public ulong UserId { get; set; }
        public ulong ConnectionId { get; set; }

        public byte[] ToBytes()
        {
            int index = 0;
            var bytes = new byte[8 * 2];
            var span = bytes.AsSpan();

            UserId.ToBytes(bytes);
            index += 8;

            ConnectionId.ToBytes(bytes.AsMemory(index));
            index += 8;

            return bytes;

        }
        public void DeBytes(ReadOnlyMemory<byte> data)
        {
            var span = data.Span;
            int index = 0;

            UserId = span.Slice(index, 8).ToUInt64();
            index += 8;

            ConnectionId = span.Slice(index, 8).ToUInt64();
            index += 8;
        }
    }
}