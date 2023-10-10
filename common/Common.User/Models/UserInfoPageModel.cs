using System;
using Common.Libs.Extends;

namespace Common.User.Models
{
    public sealed class UserInfoPageModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Account { get; set; } = string.Empty;
        public byte Sort { get; set; }

        public byte[] ToBytes()
        {
            int index = 0;
            var accountBytes = Account.GetUTF16Bytes();
            var bytes = new byte[
                8  //Page+PageSize
                + 1 //Sort
                + 2 + accountBytes.Length];
            var span = bytes.AsSpan();

            Page.ToBytes(bytes);
            index += 4;

            PageSize.ToBytes(bytes.AsMemory(index));
            index += 4;

            span[index] = Sort;
            index += 1;

            span[index] = (byte)accountBytes.Length;
            index += 1;
            span[index] = (byte)Account.Length;
            index += 1;
            accountBytes.CopyTo(span.Slice(index));

            return bytes;

        }
        public void DeBytes(ReadOnlyMemory<byte> data)
        {
            var span = data.Span;
            int index = 0;

            Page = span.Slice(index, 4).ToInt32();
            index += 4;

            PageSize = span.Slice(index, 4).ToInt32();
            index += 4;

            Sort = span[index];
            index += 1;

            Account = span.Slice(index + 2, span[index]).GetUTF16String(span[index + 1]);
            index += 2 + span[index];
        }
    }
}