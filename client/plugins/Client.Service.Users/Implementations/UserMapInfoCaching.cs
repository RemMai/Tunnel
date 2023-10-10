using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Client.Service.Users.Interfaces;
using Common.Libs.AutoInject.Attributes;
using Common.Libs.DataBase;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Service.Users.Implementations
{
    [Table("users")]
    [AutoInject(ServiceLifetime.Singleton, typeof(IUserMapInfoCaching))]
    public class UserMapInfoCaching : IUserMapInfoCaching
    {
        public ConcurrentDictionary<ulong, UserMapInfo> Users { get; set; } = new ConcurrentDictionary<ulong, UserMapInfo>();

        private readonly IConfigDataProvider<UserMapInfoCaching> configDataProvider;
        public UserMapInfoCaching() { }
        public UserMapInfoCaching(IConfigDataProvider<UserMapInfoCaching> configDataProvider)
        {
            this.configDataProvider = configDataProvider;
            UserMapInfoCaching cache = ReadConfig().Result;
            if (cache != null)
            {
                Users = cache.Users;
            }
        }

        public async Task<bool>Add(UserMapInfo map)
        {
            Users.AddOrUpdate(map.ID, map, (a, b) => map);
            await SaveConfig();
            return true;
        }
        public bool Get(ulong userid, out UserMapInfo map)
        {
            return Users.TryGetValue(userid, out map);
        }

        public async Task<UserMapInfoCaching> ReadConfig()
        {
            UserMapInfoCaching config = await configDataProvider.Load() ?? new UserMapInfoCaching();
            return config;
        }
        public async Task SaveConfig()
        {
            await configDataProvider.Save(this).ConfigureAwait(false);
        }
    }
}