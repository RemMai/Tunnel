using System.Reflection;

namespace Client.Service.Ui.api.service.Models
{
    /// <summary>
    /// 前段接口缓存
    /// </summary>
    public struct PluginPathCacheInfo
    {
        /// <summary>
        /// 对象
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// 是否void
        /// </summary>
        public bool IsVoid { get; set; }

        /// <summary>
        /// 是否task
        /// </summary>
        public bool IsTask { get; set; }

        /// <summary>
        /// 是否task result
        /// </summary>
        public bool IsTaskResult { get; set; }
    }
}