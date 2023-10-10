using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Client.Service.ForWard;

namespace client.service.forward.Models
{
    [Table("forwards")]
    public sealed class P2PConfigInfo
    {
        public P2PConfigInfo()
        {
        }

        public List<P2PListenInfo> Webs { get; set; } = new List<P2PListenInfo>();
        public List<P2PListenInfo> Tunnels { get; set; } = new List<P2PListenInfo>();
    }
}