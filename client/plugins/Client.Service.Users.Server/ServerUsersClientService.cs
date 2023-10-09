using Common.Libs.Extends;
using Common.Server;
using Common.Server.Model;
using Common.User;
using System.Threading.Tasks;
using Client.Messengers.Signin;
using Client.Service.Ui.Api.ClientServer;
using Common.Libs.AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Users.Server
{
    /// <summary>
    /// 服务器账号管理
    /// </summary>
    [AutoInject(ServiceLifetime.Singleton)]
    public sealed class ServerUsersClientService : IClientService
    {
        private readonly MessengerSender messengerSender;
        private readonly SignInStateInfo signInStateInfo;

        public ServerUsersClientService(MessengerSender messengerSender, SignInStateInfo signInStateInfo)
        {
            this.messengerSender = messengerSender;
            this.signInStateInfo = signInStateInfo;
        }

        public async Task<string> List(ClientServiceParamsInfo arg)
        {
            UserInfoPageModel page = arg.Content.DeJson<UserInfoPageModel>();
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)UsersMessengerIds.Page,
                Payload = page.ToBytes()
            });
            if (resp.Code == MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String();
            }
            return string.Empty;
        }

        public async Task<string> Add(ClientServiceParamsInfo arg)
        {
            var resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)UsersMessengerIds.Add,
                Payload = arg.Content.ToUTF8Bytes()
            });
            if (resp.Code != MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String();
            }
            return string.Empty;
        }

        public async Task<string> Password(ClientServiceParamsInfo arg)
        {
            var resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)UsersMessengerIds.Password,
                Payload = arg.Content.DeJson<UserPasswordInfo>().ToBytes()
            });
            if (resp.Code != MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String();
            }
            return string.Empty;
        }

        public async Task<string> Remove(ClientServiceParamsInfo arg)
        {
            var resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)UsersMessengerIds.Remove,
                Payload = ulong.Parse(arg.Content).ToBytes()
            });
            if (resp.Code != MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String();
            }
            return string.Empty;
        }

        public async Task<string> Info(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)UsersMessengerIds.Info
            });
            if (resp.Code == MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String();
            }
            return new UserInfo().ToJson();
        }

        public async Task<string> PasswordSelf(ClientServiceParamsInfo arg)
        {
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInStateInfo.Connection,
                MessengerId = (ushort)UsersMessengerIds.PasswordSelf,
                Payload = arg.Content.ToUTF8Bytes()
            });
            if (resp.Code != MessageResponeCodes.OK)
            {
                return resp.Data.GetUTF8String();
            }
            return string.Empty;
        }
    }
}
