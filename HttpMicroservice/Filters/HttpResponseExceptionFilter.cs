using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Commons.Models;

namespace People.HttpMicroservice.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException httpResponseException)
            {
                context.Result = new ObjectResult(new
                {
                    StatusCode = httpResponseException.StatusCode,
                    Message = httpResponseException.Message,
                    InnerException = Newtonsoft.Json.JsonConvert.SerializeObject(httpResponseException.InnerException)
                })
                {
                    StatusCode = httpResponseException.StatusCode
                };

                context.ExceptionHandled = true;
            }
            else if (context.Exception is Exception ex)
            {
                context.Result = new ObjectResult(new
                {
                    StatusCode = 500,
                    Message = "Internal Error",
                    InnerException = Newtonsoft.Json.JsonConvert.SerializeObject(ex)
                })
                {
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }
        }
    }
}

