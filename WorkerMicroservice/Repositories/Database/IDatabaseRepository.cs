using Commons.Models;

namespace WorkerMicroservice.Repositories.DataBase
{
	public interface IDatabaseRepository
	{
		Task<Person> Create(string? firstName, string? lastName, string? email, DateTime? birthday);
		Task<Person> FindById(Guid id);
		Task<IEnumerable<Person>> List(string? firstName = null, string? lastName = null, string? email = null, DateTime? birthday = null);
		Task<Person> Update(Person person, string? firstName = null, string? lastName = null, string? email = null, DateTime? birthday = null);
		Task Delete(Person person);
	}
}

