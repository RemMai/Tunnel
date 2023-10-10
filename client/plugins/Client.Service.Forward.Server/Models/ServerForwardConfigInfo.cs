using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Client.Service.ForWard.Server;

namespace client.service.forward.server.Models
{
    [Table("server-forwards")]
    public sealed class ServerForwardConfigInfo
    {
        public Dictionary<string, ServerForwardWrapInfo> List { get; set; } = new Dictionary<string, ServerForwardWrapInfo>();
    }
}