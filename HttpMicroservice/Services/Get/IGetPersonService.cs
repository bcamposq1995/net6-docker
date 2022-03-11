using Commons.Models;

namespace People.HttpMicroservice.Services.Get
{
	public interface IGetPersonService
	{
		Task<GetPersonResponse> Get(Guid id);
		Task<GetPersonListResponse> Get();
		Task<GetPersonResponse> TransactionById(Guid id);
		Task<GetPersonListResponse> TransactionList(Guid id);
	}
}

