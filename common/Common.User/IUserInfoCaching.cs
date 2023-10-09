using Common.Server;

namespace Common.User
{
    public interface IUserInfoCaching
    {
        public bool GetUser(IConnection connection, out UserInfo user);
    }
}
