using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace Northwind.WebApi.Host
{
    public class Program
    {
        private static IUnityContainer _container;
        public static void Main(string[] args)
        {
            //FunctionsAssemblyResolver.RedirectAssembly();
            // Manully create Unity container
#if DEBUG
            _container = new UnityContainer()
                // Optionally you could enable diagnostic extension
                .AddExtension(new Diagnostic());
#else
            _container = new UnityContainer();
#endif
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(
                (webHostBuilder,configurationBinder) => 
                {
                configurationBinder.AddJsonFile("appsetting.json", optional: true);
                }
            )
            .UseUnityServiceProvider(_container)
            .UseStartup<Startup>();
    }
}
