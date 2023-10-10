using Common.Server.Interfaces;
using Common.User.Models;

namespace Common.User.Interfaces
{
    public interface IUserInfoCaching
    {
        public bool GetUser(IConnection connection, out UserInfo user);
    }
}
