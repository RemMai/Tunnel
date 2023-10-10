using System;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Common.User.Enums;

namespace Common.User.Models
{
    public sealed class UserInfo
    {
        public ulong ID { get; set; }
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// 权限，0无任何权限
        /// </summary>
        public uint Access { get; set; }

        /// <summary>
        /// 限制登录数
        /// </summary>
        public int SignLimit { get; set; }
        public LimitType SignLimitType { get; set; }
        /// <summary>
        /// 登录数，只用于序列化，判断时请用 Connections.Count
        /// </summary>
        public uint SignCount { get; set; }
        [JsonIgnore]
        public bool SignLimitDenied => SignLimitType == LimitType.Limit && Connections.Count >= SignLimit;
        [JsonIgnore]
        public ConcurrentDictionary<ulong, UserConnectionWrap> Connections { get; set; } = new ConcurrentDictionary<ulong, UserConnectionWrap>();

        /// <summary>
        /// 限制流量 
        /// </summary>
        public long NetFlow { get; set; }
        public LimitType NetFlowType { get; set; }
        public ulong SentBytes { get; set; }
        [JsonIgnore]
        public bool NetFlowDenied => NetFlowType == LimitType.Limit && NetFlow <= 0;

        /// <summary>
        /// 账号添加时间
        /// </summary>
        public DateTime AddTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 账号结束时间
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.Now;


        public const byte SortID = 0b0000_0001;
        public const byte SortAddTime = 0b0000_0010;
        public const byte SortEndTime = 0b0000_0100;
        public const byte SortNetFlow = 0b0000_1000;
        public const byte SortSignLimit = 0b0001_0000;
        public const byte SortAsc = 0b0000_00000;
        public const byte SortDesc = 0b0000_0001;
    }
}
