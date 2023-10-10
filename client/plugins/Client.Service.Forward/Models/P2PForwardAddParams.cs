using Client.Service.ForWard;

namespace client.service.forward.Models;

public sealed class P2PForwardAddParams
{
    public uint ListenID { get; set; }
    public P2PForwardInfo Forward { get; set; } = new P2PForwardInfo();
}