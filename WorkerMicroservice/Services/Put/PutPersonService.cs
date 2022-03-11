using Commons.Models;
using WorkerMicroservice.Repositories.DataBase;
using Prometheus;

namespace WorkerMicroservice.Services.Put
{
    public class PutPersonService : IPutPersonService
    {
        private readonly IDatabaseRepository _dbRepository;

        public PutPersonService(IDatabaseRepository repository)
        {
            this._dbRepository = repository;
        }

        /// <summary>
        /// Update specific attributes, only the not nulled attributes are asigned
        /// </summary>
        /// <param name="id">The item to be modified</param>
        /// <param name="request">The new values</param>
        /// <returns>The person model</returns>
        /// <exception cref="HttpResponseException"></exception>
        public async Task<Person> Put(Guid id, PutPersonRequest request)
        {
            var foundItem = await this._dbRepository.FindById(id);
            if (foundItem == null) throw new WorkerResponseException(404, $"The person with id {id} was not found");
            if (request != null)
            {
                foundItem = await this._dbRepository.Update(foundItem,
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    email: request.Email,
                    birthday: request.Birthday);
            }
            return foundItem;
        }
    }
}

