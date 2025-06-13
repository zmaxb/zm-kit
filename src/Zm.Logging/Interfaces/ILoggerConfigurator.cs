using Microsoft.Extensions.Configuration;

namespace Zm.Logging.Interfaces;

public interface ILoggerConfigurator
{
    void Configure(string serviceName, IConfiguration configuration);
}