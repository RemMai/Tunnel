﻿using client.messengers.clients;
using client.messengers.register;
using client.service.socks5;
using common.libs;
using common.libs.extends;
using common.server;
using common.socks5;
using System;
using System.Linq;
using System.Net;

namespace client.service.vea
{
    /// <summary>
    /// 组网socks5客户端
    /// </summary>
    public interface IVeaSocks5ClientHandler : ISocks5ClientHandler
    {
    }

    /// <summary>
    /// 组网socks5客户端
    /// </summary>
    public sealed class VeaSocks5ClientHandler : Socks5ClientHandler, IVeaSocks5ClientHandler
    {
        private readonly IVeaSocks5MessengerSender socks5MessengerSender;
        private readonly Config config;
        private readonly IClientInfoCaching clientInfoCaching;
        private readonly VeaTransfer virtualEthernetAdapterTransfer;
        IVeaSocks5ClientListener socks5ClientListener;

        /// <summary>
        /// 组网socks5客户端
        /// </summary>
        /// <param name="socks5MessengerSender"></param>
        /// <param name="registerStateInfo"></param>
        /// <param name="config"></param>
        /// <param name="clientInfoCaching"></param>
        /// <param name="socks5ClientListener"></param>
        /// <param name="virtualEthernetAdapterTransfer"></param>
        public VeaSocks5ClientHandler(IVeaSocks5MessengerSender socks5MessengerSender, RegisterStateInfo registerStateInfo, Config config, IClientInfoCaching clientInfoCaching, IVeaSocks5ClientListener socks5ClientListener, VeaTransfer virtualEthernetAdapterTransfer)
            : base(socks5MessengerSender, registerStateInfo, new common.socks5.Config
            {
                ConnectEnable = config.ConnectEnable,
                NumConnections = config.NumConnections,
                BufferSize = config.BufferSize,
                TargetName = config.TargetName,
            }, clientInfoCaching, socks5ClientListener)
        {
            this.socks5MessengerSender = socks5MessengerSender;
            this.config = config;
            this.clientInfoCaching = clientInfoCaching;
            this.socks5ClientListener = socks5ClientListener;
            this.virtualEthernetAdapterTransfer = virtualEthernetAdapterTransfer;
        }

        /// <summary>
        /// 收到关闭
        /// </summary>
        /// <param name="info"></param>
        protected override void OnClose(Socks5Info info)
        {
            if (info.Tag is TagInfo target)
            {
                socks5MessengerSender.RequestClose(info.Id, target.Connection);
            }
        }

        /// <summary>
        /// 命令
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool HandleCommand(Socks5Info data)
        {
            if ((data.Tag is TagInfo target) == false)
            {
                target = new TagInfo();
                data.Tag = target;
            }
            var targetEp = Socks5Parser.GetRemoteEndPoint(data.Data, out Span<byte> ipMemory);

            target.TargetIp = targetEp.Address;
            if (targetEp.Port == 0 || ipMemory.SequenceEqual(Helper.AnyIpArray))
            {
                data.Response[0] = (byte)Socks5EnumResponseCommand.ConnecSuccess;
                data.Data = data.Response.AsMemory(0,1);

                CommandResponseData(data);
                socks5ClientListener.Response(data);
                return true;
            }
            target.Connection = GetConnection(target.TargetIp, ipMemory);

            return socks5MessengerSender.Request(data, target.Connection);
        }

        /// <summary>
        /// 转发
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool HndleForward(Socks5Info data)
        {
            TagInfo target = data.Tag as TagInfo;

            if (target.Connection == null || target.Connection.Connected == false)
            {
                return false;
            }

            return socks5MessengerSender.Request(data, target.Connection);
        }

        /// <summary>
        /// udp转发
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool HndleForwardUdp(Socks5Info data)
        {
            IPEndPoint remoteEndPoint = Socks5Parser.GetRemoteEndPoint(data.Data, out Span<byte> ipMemory);
            IConnection connection = GetConnection(remoteEndPoint.Address, ipMemory);
            return socks5MessengerSender.Request(data, connection);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public override void Flush()
        {
        }

        private int[] Masks = new int[] { 0xffffff, 0xffff, 0xff };
        private IConnection GetConnection(IPAddress target, Span<byte> ipMemory)
        {
            if (virtualEthernetAdapterTransfer.IPList.TryGetValue(target, out IPAddressCacheInfo cache))
            {
                return cache.Client.Connection;
            }

            if (target.IsLan())
            {
                int ip = ipMemory.ToInt32();
                for (int i = 0; i < Masks.Length; i++)
                {
                    if (virtualEthernetAdapterTransfer.LanIPList.TryGetValue(ip & Masks[i], out cache))
                    {
                        return cache.Client.Connection;
                    }
                }
            }
;
            if (clientInfoCaching.GetByName(config.TargetName, out ClientInfo client))
            {
                return client.Connection;
            }
            return null;
        }
        sealed class TagInfo
        {
            public IConnection Connection { get; set; }
            public IPAddress TargetIp { get; set; }
        }
    }
}
