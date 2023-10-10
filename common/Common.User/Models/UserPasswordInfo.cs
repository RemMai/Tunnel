using System;
using Common.Libs.Extends;

namespace Common.User.Models
{
    public sealed class UserPasswordInfo
    {
        public ulong ID { get; set; }
        public string Password { get; set; }

        public byte[] ToBytes()
        {
            int index = 0;
            var passwordBytes = Password.GetUTF16Bytes();
            var bytes = new byte[
                8  //ID
                + 2 + passwordBytes.Length];
            var span = bytes.AsSpan();

            ID.ToBytes(bytes);
            index += 8;

            span[index] = (byte)passwordBytes.Length;
            index += 1;
            span[index] = (byte)Password.Length;
            index += 1;
            passwordBytes.CopyTo(span.Slice(index));

            return bytes;

        }
        public void DeBytes(ReadOnlyMemory<byte> data)
        {
            var span = data.Span;
            int index = 0;

            ID = span.Slice(index, 8).ToUInt64();
            index += 8;

            Password = span.Slice(index + 2, span[index]).GetUTF16String(span[index + 1]);
            index += 2 + span[index];
        }
    }
}