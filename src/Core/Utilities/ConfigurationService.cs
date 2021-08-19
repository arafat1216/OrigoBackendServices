using Microsoft.Extensions.Configuration;

namespace Common.Utilities
{
    class ConfigurationService
    {
        protected readonly IConfiguration Configuration;

        public ConfigurationService()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json",
                             optional: false,
                             reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<ConfigurationService>();

            Configuration = builder.Build();
        }

        public IConfiguration GetConfiguration()
        {
            return Configuration;
        }
    }
}
