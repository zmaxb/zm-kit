using Microsoft.Extensions.Configuration;
using Zm.Logging.Interfaces;
using Zm.Logging.Static;

namespace Zm.Logging.Internal;

public class SerilogConfigurator : ILoggerConfigurator
{
    public void Configure(string serviceName, IConfiguration configuration)
    {
        Logger.ConfigureLogging(serviceName, configuration);
    }
}