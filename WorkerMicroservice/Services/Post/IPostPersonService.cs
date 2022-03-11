using Commons.Models;

namespace WorkerMicroservice.Services.Post
{
	public interface IPostPersonService
	{
		Task<Person> Post(PostPersonRequest request);
	}
}

