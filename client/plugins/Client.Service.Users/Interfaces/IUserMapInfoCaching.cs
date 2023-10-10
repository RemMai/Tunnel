using System.Threading.Tasks;

namespace Client.Service.Users.Interfaces
{
    public interface IUserMapInfoCaching
    {
        public Task<bool> Add(UserMapInfo map);
        public bool Get(ulong userid, out UserMapInfo map);
    }
}
