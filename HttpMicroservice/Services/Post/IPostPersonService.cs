using Commons.Models;

namespace People.HttpMicroservice.Services.Post
{
	public interface IPostPersonService
	{
		Task<PostPersonResponse> Post(PostPersonRequest request);
		Task<PostPersonResponse> Transaction(Guid id);
	}
}

