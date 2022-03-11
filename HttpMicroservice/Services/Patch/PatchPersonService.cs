using Commons.Models;
using HttpMicroservice.Repositories.Cache;
using HttpMicroservice.Repositories.Queue;

namespace People.HttpMicroservice.Services.Patch
{
    public class PatchPersonService : IPatchPersonService
    {

        private readonly IQueueRepository _queueRepository;
        private readonly ICacheRepository _cacheRepository;

        public PatchPersonService(IQueueRepository queueRepository, ICacheRepository cacheRepository)
        {
            this._cacheRepository = cacheRepository;
            this._queueRepository = queueRepository;
        }

        /// <summary>
        /// Update the whole entity
        /// </summary>
        /// <param name="id">The entity id</param>
        /// <param name="request">The request object</param>
        /// <returns>PatchPersonResponse</returns>
        public async Task<PatchPersonResponse> Patch(Guid id, PatchPersonRequest request)
        {
            PatchPersonResponse response = new PatchPersonResponse
            {
                DateTime = DateTime.Now,
                Status = TransactionStatus.PENDING,
                TransactionId = Guid.NewGuid(),
                Request = request,
                PersonId = id
            };

            await this._cacheRepository.SaveObject(response.TransactionId.ToString(), response, 24);
            this._queueRepository.Enqueue(response, "patch");

            return response;
        }

        public async Task<PatchPersonResponse> Transaction(Guid id) => (await this._cacheRepository.FindById<PatchPersonResponse>(id))!;
    }
}

