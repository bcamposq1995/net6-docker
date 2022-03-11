using HttpMicroservice.Repositories.Queue;
using Commons.Models;
using HttpMicroservice.Repositories.Cache;

namespace People.HttpMicroservice.Services.Delete
{
	public class DeletePersonService : IDeletePersonService
	{
        private readonly IQueueRepository _queueRepository;
        private readonly ICacheRepository _cacheRepository;

        public DeletePersonService(IQueueRepository queueRepository, ICacheRepository cacheRepository)
        {
            this._cacheRepository = cacheRepository;
            this._queueRepository = queueRepository;
        }

        /// <summary>
        /// Delete an existing person
        /// </summary>
        /// <param name="id">The person's id</param>
        /// <returns>Nothing it is an async function</returns>
        /// <exception cref="HttpResponseException">Throws a 404 err if the person id does not exist</exception>
        public async Task<DeletePersonResponse> Delete(Guid id)
        {
            DeletePersonResponse response = new DeletePersonResponse
            {
                DateTime = DateTime.Now,
                Status = TransactionStatus.PENDING,
                TransactionId = Guid.NewGuid(),
                PersonId = id
            };

            await this._cacheRepository.SaveObject(response.TransactionId.ToString(), response, 24);
            this._queueRepository.Enqueue(response, "delete");

            return response;
        }

        public async Task<DeletePersonResponse> Transaction(Guid id) => (await this._cacheRepository.FindById<DeletePersonResponse>(id))!;
    }
}

