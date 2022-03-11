using Commons.Models;

namespace WorkerMicroservice.Services.Patch
{
	public interface IPatchPersonService
	{
		Task<Person> Patch(Guid id, PatchPersonRequest request);
	}
}

