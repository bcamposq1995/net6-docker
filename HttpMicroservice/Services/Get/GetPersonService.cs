using Commons.Models;
using HttpMicroservice.Repositories.Cache;
using HttpMicroservice.Repositories.Queue;

namespace People.HttpMicroservice.Services.Get
{
    public class GetPersonService : IGetPersonService
    {

        private readonly IQueueRepository _queueRepository;
        private readonly ICacheRepository _cacheRepository;

        public GetPersonService(IQueueRepository queueRepository, ICacheRepository cacheRepository)
        {
            this._cacheRepository = cacheRepository;
            this._queueRepository = queueRepository;
        }

        public async Task<GetPersonResponse> Get(Guid id)
        {
            GetPersonResponse response = new GetPersonResponse
            {
                DateTime = DateTime.Now,
                Status = TransactionStatus.PENDING,
                TransactionId = Guid.NewGuid()
            };

            await this._cacheRepository.SaveObject(response.TransactionId.ToString(), response, 1);
            this._queueRepository.Enqueue(response, "get-by-id");

            return response;
        }

        public async Task<GetPersonListResponse> Get()
        {
            GetPersonListResponse response = new GetPersonListResponse
            {
                DateTime = DateTime.Now,
                Status = TransactionStatus.PENDING,
                TransactionId = Guid.NewGuid()
            };

            await this._cacheRepository.SaveObject(response.TransactionId.ToString(), response);
            this._queueRepository.Enqueue(response, "get-list");

            return response;
        }

        public async Task<GetPersonResponse> TransactionById(Guid id) => (await this._cacheRepository.FindById<GetPersonResponse>(id))!;

        public async Task<GetPersonListResponse> TransactionList(Guid id) => (await this._cacheRepository.FindById<GetPersonListResponse>(id))!;
    }
}

