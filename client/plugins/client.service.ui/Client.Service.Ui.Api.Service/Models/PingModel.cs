using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace client.service.ui.api.service.Models
{
    class PingModel
    {
        public Ping Ping { get; set; }
        public Task<PingReply> Task { get; set; }

    }
}