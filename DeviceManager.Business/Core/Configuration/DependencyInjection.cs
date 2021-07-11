using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PolicyDomain.Business.Core.Behaviours;
using System.Reflection;

namespace DeviceManager.Business.Core.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly())
                    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
