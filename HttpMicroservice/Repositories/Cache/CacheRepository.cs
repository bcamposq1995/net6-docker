using HttpMicroservice.Repositories.Cache;
using StackExchange.Redis;

namespace People.HttpMicroservice.Repositories.Cache
{
	public class CacheRepository : ICacheRepository
	{
        private readonly IDatabase _database;

        public CacheRepository(IDatabase database)
		{
            this._database = database;
		}

        public async Task<T?> FindById<T>(Guid id)
        {
            string json = await this._database.StringGetAsync(id.ToString());
            if (json != null) return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            return default;
        }

        public async Task<IEnumerable<T>?> List<T>()
        {
            string json = await this._database.StringGetAsync("list");
            if (json != null) return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json);
            return null;
        }

        public async Task SaveObject<T>(string name, T obj, int hoursCache = 1)
        {
            this._database.StringGetDelete(new RedisKey(name));
            await this._database.StringSetAsync(name, Newtonsoft.Json.JsonConvert.SerializeObject(obj), expiry: new TimeSpan(DateTime.Now.AddHours(1).Ticks));
        }
    }
}

