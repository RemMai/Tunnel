﻿using Client.Service.ForWard;

namespace Client.Service.forward.Models;

public sealed class P2PForwardAddParams
{
    public uint ListenID { get; set; }
    public P2PForwardInfo Forward { get; set; } = new P2PForwardInfo();
}