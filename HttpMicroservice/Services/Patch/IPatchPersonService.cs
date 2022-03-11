using Commons.Models;

namespace People.HttpMicroservice.Services.Patch
{
	public interface IPatchPersonService
	{
		Task<PatchPersonResponse> Patch(Guid id, PatchPersonRequest request);
		Task<PatchPersonResponse> Transaction(Guid id);
	}
}

