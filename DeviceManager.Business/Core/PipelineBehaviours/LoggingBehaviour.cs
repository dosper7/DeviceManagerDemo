
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace PolicyDomain.Business.Core.Behaviours
{

    internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                _logger.LogInformation($"   Handling {typeof(TRequest).Name} with value: {{@request}}", request);
                var response = await next().ConfigureAwait(false);
                _logger.LogInformation($"   Handled {typeof(TResponse).Name} with value: {{@response}}", response);
                return response;
            }
            catch (System.Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogError(ex, "Unhandled Exception for Request {Name} {@Request}", requestName, request);
                throw;
            }

        }
    }
}