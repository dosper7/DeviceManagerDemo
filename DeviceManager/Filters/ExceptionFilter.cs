using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace PolicyDomain.API.Filter
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context?.Exception?.ToString());
            var rsp = new ApiResult();

            context.Result = new ObjectResult(rsp.AddError(context.Exception.ToString()));
            context.HttpContext.Response.StatusCode = 500;
            context.ExceptionHandled = true;

        }




    }
}
