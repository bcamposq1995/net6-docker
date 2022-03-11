namespace WorkerMicroservice.Repositories.Cache
{
	public interface ICacheRepository
	{
		Task<T?> FindById<T>(Guid id);
		Task<IEnumerable<T>?> List<T>();
		Task SaveObject<T>(string name, T obj, int hoursCache = 1);
	}
}

