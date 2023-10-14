using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Client.Service.Ui.api.service.Models
{
    class PingModel
    {
        public Ping Ping { get; set; }
        public Task<PingReply> Task { get; set; }

    }
}