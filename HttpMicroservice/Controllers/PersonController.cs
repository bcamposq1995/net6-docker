using People.HttpMicroservice.Filters;
using People.HttpMicroservice.Services.Delete;
using People.HttpMicroservice.Services.Get;
using People.HttpMicroservice.Services.Patch;
using People.HttpMicroservice.Services.Post;
using People.HttpMicroservice.Services.Put;
using Microsoft.AspNetCore.Mvc;
using Commons.Models;

namespace HttpMicroservice.Controllers
{
    [Route("/")]
    public class PersonController : Controller
    {
        [HttpHead]
        public IActionResult Get() => new OkResult();

        [HttpGet("{id}")]
        [ResponseCache(Duration = 60)]
        public async Task<GetPersonResponse> Get([FromServices] IGetPersonService service, [FromRoute] Guid id) => await service.Get(id);

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<GetPersonListResponse> Get([FromServices] IGetPersonService service) => await service.Get();

        [HttpPost]
        [ModelValidatorFilter]
        public async Task<IActionResult> Post([FromServices] IPostPersonService service,
            [FromBody] PostPersonRequest request) =>
            new CreatedResult($"transaction/post/{(await service.Post(request)).TransactionId}", null);

        [HttpPut("{id}")]
        public async Task<PutPersonResponse> Put([FromServices] IPutPersonService service,
            [FromRoute] Guid id, [FromBody] PutPersonRequest request) =>
            await service.Put(id, request);

        [HttpPatch("{id}")]
        [ModelValidatorFilter]
        public async Task<PatchPersonResponse> Patch([FromServices] IPatchPersonService service,
            [FromRoute] Guid id, [FromBody] PatchPersonRequest request) =>
            await service.Patch(id, request);

        [HttpDelete("{id}")]
        public async Task Delete([FromServices] IDeletePersonService service,
            [FromRoute] Guid id) =>
            await service.Delete(id);
    }
}

