using Commons.Models;
using WorkerMicroservice.Repositories.DataBase;

namespace WorkerMicroservice.Services.Delete
{
	public class DeletePersonService : IDeletePersonService
	{
        private readonly IDatabaseRepository _dbRepository;

        public DeletePersonService(IDatabaseRepository repository)
        {
            this._dbRepository = repository;
        }

        /// <summary>
        /// Delete an existing person
        /// </summary>
        /// <param name="id">The person's id</param>
        /// <returns>Nothing it is an async function</returns>
        /// <exception cref="HttpResponseException">Throws a 404 err if the person id does not exist</exception>
        public async Task Delete(Guid id)
        {
            var foundItem = await this._dbRepository.FindById(id);
            if (foundItem == null) throw new WorkerResponseException(404, $"The person with id {id} was not found");
            await this._dbRepository.Delete(foundItem);
        }
    }
}

