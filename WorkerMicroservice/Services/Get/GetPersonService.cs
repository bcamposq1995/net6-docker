using Commons.Models;
using WorkerMicroservice.Repositories.DataBase;

namespace People.HttpMicroservice.Services.Get
{
    public class GetPersonService : IGetPersonService
    {

        private readonly IDatabaseRepository _dbRepository;

        public GetPersonService(IDatabaseRepository repository)
        {
            this._dbRepository = repository;
        }

        public async Task<Person> Get(Guid id) => await this._dbRepository.FindById(id) ?? throw new HttpResponseException(404, "Person not found");
        public async Task<List<Person>> Get() => (await this._dbRepository.List()).ToList() ?? throw new HttpResponseException(404, "The people table is empty");
    }
}

