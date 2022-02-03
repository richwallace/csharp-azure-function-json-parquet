using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace EventToADLSParquet
{
    public class Startup
    {
        public void Configure(IFunctionsHostBuilder builder)
        {
            if (null == builder) throw new ArgumentNullException(nameof(builder));
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
    }
}