using Microsoft.Extensions.DependencyInjection;
using Zm.Logging.Interfaces;
using Zm.Logging.Internal;
using Zm.Logging.Services;

namespace Zm.Logging.Extensions;

public static class ServiceCollectionExtensions
{
    // ReSharper disable once InconsistentNaming
    public static IServiceCollection AddZmLogging(this IServiceCollection services)
    {
        services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));
        services.AddSingleton<ILoggerConfigurator, SerilogConfigurator>();
        return services;
    }
}