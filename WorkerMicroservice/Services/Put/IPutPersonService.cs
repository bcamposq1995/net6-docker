using Commons.Models;

namespace WorkerMicroservice.Services.Put
{
	public interface IPutPersonService
	{
		Task<Person> Put(Guid id, PutPersonRequest request);
	}
}

