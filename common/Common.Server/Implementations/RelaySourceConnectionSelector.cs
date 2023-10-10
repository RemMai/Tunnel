using Common.Server;
using Common.Server.Interfaces;

namespace Common.Server.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RelaySourceConnectionSelector : IRelaySourceConnectionSelector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public IConnection Select(IConnection connection, ulong clientid) => connection;
    }
}