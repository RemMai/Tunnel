using System.Collections.Generic;

namespace Common.User.Models
{
    public sealed class UserInfoPageResultModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<UserInfo> Data { get; set; }
    }
}