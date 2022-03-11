using Commons.Models;
using WorkerMicroservice.Repositories.DataBase;

namespace WorkerMicroservice.Services.Patch
{
    public class PatchPersonService : IPatchPersonService
    {

        private readonly IDatabaseRepository _dbRepository;

        public PatchPersonService(IDatabaseRepository repository)
        {
            this._dbRepository = repository;
        }

        /// <summary>
        /// Update the whole entity
        /// </summary>
        /// <param name="id">The item id</param>
        /// <param name="request">The whole entity to update</param>
        /// <returns>The updated person</returns>
        /// <exception cref="HttpResponseException">If it does not exist, throws a 404 error</exception>
        public async Task<Person> Patch(Guid id, PatchPersonRequest request)
        {
            var foundItem = await this._dbRepository.FindById(id);
            if (foundItem == null) throw new WorkerResponseException(404, $"The person with id {id} was not found");
            return await this._dbRepository.Update(foundItem,
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    email: request.Email,
                    birthday: request.Birthday);
        }
    }
}

