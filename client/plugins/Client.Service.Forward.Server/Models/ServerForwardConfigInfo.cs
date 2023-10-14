using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Client.Service.ForWard.Server;

namespace Client.Service.ForWard.Server.Models
{
    [Table("server-forwards")]
    public sealed class ServerForwardConfigInfo
    {
        public Dictionary<string, ServerForwardWrapInfo> List { get; set; } = new Dictionary<string, ServerForwardWrapInfo>();
    }
}