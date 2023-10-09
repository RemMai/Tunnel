﻿using Common.Server.Model;
using System.Threading.Tasks;

namespace Client.Messengers.Signin
{
    /// <summary>
    /// 注册接口
    /// </summary>
    public interface ISignInTransfer
    {
        /// <summary>
        /// 退出
        /// </summary>
        void Exit();
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="autoReg">强制自动注册</param>
        /// <returns></returns>
        Task<CommonTaskResponseInfo<bool>> SignIn(bool autoReg = false);
    }
}
