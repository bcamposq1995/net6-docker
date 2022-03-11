using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace People.HttpMicroservice.Filters
{
	public class ModelValidatorFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    StatusCode = 400,
                    Message = "Invalid request model",
                    Data = (from modelState in context.ModelState.Values from error in modelState.Errors select error.ErrorMessage).ToList()
                });
            }
        }
    }
}

