using System;
using Common.Server.Attributes;

namespace Common.User.Enums
{
    /// <summary>
    /// 权限相关的消息id
    /// </summary>
    [Flags, MessengerIdEnum]
    public enum UsersMessengerIds : ushort
    {
        Min = 1000,
        /// <summary>
        /// 分页
        /// </summary>
        Page = 1001,
        /// <summary>
        /// 添加修改
        /// </summary>
        Add = 1002,
        /// <summary>
        /// 更新密码
        /// </summary>
        Password = 1003,
        PasswordSelf = 1004,
        /// <summary>
        /// 删除
        /// </summary>
        Remove = 1005,
        /// <summary>
        /// 信息
        /// </summary>
        Info = 1006,

        /// <summary>
        /// 获取配置
        /// </summary>
        GetSetting = 1007,
        /// <summary>
        /// 设置配置
        /// </summary>
        Setting = 1008,

        /// <summary>
        /// 验证登入
        /// </summary>
        SignIn = 1009,

        Max = 1099,
    }
}