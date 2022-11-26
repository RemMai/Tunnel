﻿using client.messengers.punchHole;
using common.libs;
using System;

namespace client.realize.messengers.punchHole
{
    public class PunchHoleReverse : IPunchHole
    {
        private readonly PunchHoleMessengerSender punchHoleMessengerSender;
        public PunchHoleReverse(PunchHoleMessengerSender punchHoleMessengerSender)
        {

            this.punchHoleMessengerSender = punchHoleMessengerSender;
        }

        public PunchHoleTypes Type => PunchHoleTypes.REVERSE;

        public void Execute(OnPunchHoleArg arg)
        {
            punchHoleMessengerSender.OnReverse.Push(arg);
        }
    }
}
