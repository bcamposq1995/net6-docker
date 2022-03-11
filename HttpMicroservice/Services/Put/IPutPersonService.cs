using Commons.Models;

namespace People.HttpMicroservice.Services.Put
{
	public interface IPutPersonService
	{
		Task<PutPersonResponse> Put(Guid id, PutPersonRequest request);
		Task<PutPersonResponse> Transaction(Guid id);
	}
}

