using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace OrderManagement.Infrastructure.Extensions
{
    public static class LoggingExtensions
    {
        public static IHostBuilder AddSerilogLogging(
            this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog(
                (context, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(
                            context.Configuration)
                        .Enrich.FromLogContext();
                });

            return hostBuilder;
        }
    }
}
