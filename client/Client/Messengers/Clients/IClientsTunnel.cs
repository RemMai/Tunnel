using Common.Server;
using System;
using System.Threading.Tasks;
using Common.Server.Interfaces;
using Common.Server.Models;

namespace Client.Messengers.Clients
{
    /// <summary>
    /// 客户端打洞新通道
    /// </summary>
    public interface IClientsTunnel
    {
        /// <summary>
        /// 通道关闭
        /// </summary>
        public Action<IConnection, IConnection> OnDisConnect { get; set; }
        /// <summary>
        /// 新通道
        /// </summary>
        /// <param name="serverType"></param>
        /// <param name="selfId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public Task<ushort> NewBind(ServerType serverType, ulong selfId, ulong targetId);
    }
}
