using Common.Libs.Extends;
using Common.Server;
using Common.Server.Model;
using Common.User;
using System.Threading.Tasks;
using Client.Messengers.clients;
using Client.Messengers.Signin;
using Client.Service.Ui.Api.ClientServer;

namespace Client.Service.Users
{
    /// <summary>
    /// 服务器账号管理
    /// </summary>
    public sealed class UsersClientService : IClientService
    {
        private readonly MessengerSender messengerSender;
        private readonly SignInStateInfo signInStateInfo;
        private readonly IUserMapInfoCaching userMapInfoCaching;
        private readonly IClientInfoCaching clientInfoCaching;

        public UsersClientService(MessengerSender messengerSender, SignInStateInfo signInStateInfo, IUserMapInfoCaching userMapInfoCaching, IClientInfoCaching clientInfoCaching)
        {
            this.messengerSender = messengerSender;
            this.signInStateInfo = signInStateInfo;
            this.userMapInfoCaching = userMapInfoCaching;
            this.clientInfoCaching = clientInfoCaching;
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
                UserInfoPageResultModel pageResult = resp.Data.GetUTF8String().DeJson<UserInfoPageResultModel>();
                foreach (UserInfo item in pageResult.Data)
                {
                    if (userMapInfoCaching.Get(item.ID, out UserMapInfo map))
                    {
                        item.Access = map.Access;
                    }
                    else
                    {
                        item.Access = (uint)EnumServiceAccess.None;
                    }
                }
                return pageResult.ToJson();
            }

            return string.Empty;
        }

        public async Task<bool> Update(ClientServiceParamsInfo arg)
        {
            UserMapInfo map = arg.Content.DeJson<UserMapInfo>();
            if (userMapInfoCaching.Get(map.ID, out UserMapInfo mapInfo))
            {
                mapInfo.Access = map.Access;
                if (clientInfoCaching.Get(mapInfo.ConnectionId, out ClientInfo client))
                {
                    client.UserAccess = mapInfo.Access;
                }
            }
            else
            {
                await userMapInfoCaching.Add(map);
            }
            return true;
        }
    }

}
