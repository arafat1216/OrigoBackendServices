using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    class AzureKeyVaultService
    {
        private readonly IConfiguration Configuration;

        public AzureKeyVaultService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
