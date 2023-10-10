using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.User.Models
{
    [Table("users")]
    public sealed class UserStoreModel
    {
        public Dictionary<ulong, UserInfo> Users { get; set; }
    }
}