using Commons.Models;

namespace People.HttpMicroservice.Services.Get
{
	public interface IGetPersonService
	{
		Task<Person> Get(Guid id);
		Task<List<Person>> Get();
	}
}

