using Commons.Models;
using Microsoft.AspNetCore.Mvc;
using People.HttpMicroservice.Services.Delete;
using People.HttpMicroservice.Services.Get;
using People.HttpMicroservice.Services.Patch;
using People.HttpMicroservice.Services.Post;
using People.HttpMicroservice.Services.Put;

namespace HttpMicroservice.Controllers
{
    [Route("transaction")]
    public class TransactionController : Controller
    {
        [HttpGet("post/{id}")]
        public async Task<PostPersonResponse> Post([FromServices] IPostPersonService service, [FromRoute] Guid id) => await service.Transaction(id);

        [HttpGet("put/{id}")]
        public async Task<PutPersonResponse> Put([FromServices] IPutPersonService service, [FromRoute] Guid id) => await service.Transaction(id);

        [HttpGet("patch/{id}")]
        public async Task<PatchPersonResponse> Patch([FromServices] IPatchPersonService service, [FromRoute] Guid id) => await service.Transaction(id);

        [HttpGet("delete/{id}")]
        public async Task<DeletePersonResponse> Delete([FromServices] IDeletePersonService service, [FromRoute] Guid id) => await service.Transaction(id);

        [HttpGet("get-by-id/{id}")]
        public async Task<GetPersonResponse> GetById([FromServices] IGetPersonService service, [FromRoute] Guid id) => await service.TransactionById(id);

        [HttpGet("get-list/{id}")]
        public async Task<GetPersonListResponse> GetList([FromServices] IGetPersonService service, [FromRoute] Guid id) => await service.TransactionList(id);
    }
}