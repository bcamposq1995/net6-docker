using Commons.Models;
using HttpMicroservice.Repositories.Cache;
using HttpMicroservice.Repositories.Queue;

namespace People.HttpMicroservice.Services.Put
{
    public class PutPersonService : IPutPersonService
    {
        private readonly IQueueRepository _queueRepository;
        private readonly ICacheRepository _cacheRepository;

        public PutPersonService(IQueueRepository queueRepository, ICacheRepository cacheRepository)
        {
            this._cacheRepository = cacheRepository;
            this._queueRepository = queueRepository;
        }

        /// <summary>
        /// Update specific attributes, only the not nulled attributes are asigned
        /// </summary>
        /// <param name="id">Guid Id</param>
        /// <param name="request">PutPersonRequest</param>
        /// <returns>PutPersonResponse</returns>
        public async Task<PutPersonResponse> Put(Guid id, PutPersonRequest request)
        {
            PutPersonResponse response = new()
            {
                DateTime = DateTime.Now,
                Status = TransactionStatus.PENDING,
                TransactionId = Guid.NewGuid(),
                Request = request,
                PersonId = id
            };

            await this._cacheRepository.SaveObject(response.TransactionId.ToString(), response, 24);
            this._queueRepository.Enqueue(response, "put");

            return response;
        }

        public async Task<PutPersonResponse> Transaction(Guid id) => (await this._cacheRepository.FindById<PutPersonResponse>(id))!;
    }
}

