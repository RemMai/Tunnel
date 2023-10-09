using Common.Libs;
using Common.Libs.Extends;
using Common.Server;
using Common.Vea;
using System;
using System.Threading.Tasks;
using Client.Messengers.Signin;
using Client.Service.Ui.Api.ClientServer;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Vea.Server
{
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ServerVeaClientService : IClientService
    {
        private readonly MessengerSender messengerSender;
        private readonly SignInStateInfo signInStateInfo;

        public ServerVeaClientService(MessengerSender messengerSender, SignInStateInfo signInStateInfo)
        {
            this.messengerSender = messengerSender;
            this.signInStateInfo = signInStateInfo;
        }

        public async Task<DHCPInfo> List(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new Common.Server.Model.MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)Common.Vea.VeaSocks5MessengerIds.Network,
            });
            if (resp.Code == Common.Server.Model.MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String().DeJson<DHCPInfo>();
            }

            return null;
        }

        public async Task<bool> AddNetwork(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new Common.Server.Model.MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)Common.Vea.VeaSocks5MessengerIds.AddNetwork,
                Payload = BitConverter.GetBytes(uint.Parse(arg.Content))
            });
            if (resp.Code == Common.Server.Model.MessageResponeCodes.OK)
            {
                return resp.Data.Span.SequenceEqual(Helper.TrueArray);
            }

            return false;
        }

        public async Task<bool> ModifyIP(ClientServiceParamsInfo arg)
        {
            ModifyIPModel model = arg.Content.DeJson<ModifyIPModel>();
            MessageResponeInfo resp = await messengerSender.SendReply(new Common.Server.Model.MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)Common.Vea.VeaSocks5MessengerIds.ModifyIP,
                Payload = model.ToBytes()
            });
            if (resp.Code == Common.Server.Model.MessageResponeCodes.OK)
            {
                return resp.Data.Span.SequenceEqual(Helper.TrueArray);
            }

            return false;
        }

        public async Task<bool> DeleteIP(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new Common.Server.Model.MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)Common.Vea.VeaSocks5MessengerIds.DeleteIP,
                Payload = BitConverter.GetBytes(ulong.Parse(arg.Content))
            });
            if (resp.Code == Common.Server.Model.MessageResponeCodes.OK)
            {
                return resp.Data.Span.SequenceEqual(Helper.TrueArray);
            }

            return false;
        }
    }

    public sealed class ModifyIPModel
    {
        public ulong ConnectionId { get; set; }
        public byte IP { get; set; }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[8 + 1];
            ConnectionId.ToBytes(bytes);
            bytes[8] = IP;
            return bytes;
        }
    }
}