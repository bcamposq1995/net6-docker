using Commons.Models;

namespace People.HttpMicroservice.Services.Delete
{
	public interface IDeletePersonService
	{
		Task<DeletePersonResponse> Delete(Guid id);
		Task<DeletePersonResponse> Transaction(Guid id);
	}
}

