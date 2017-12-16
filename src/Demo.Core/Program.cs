using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Demo.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
               .UseKestrel()
               .UseIISIntegration()
               .UseContentRoot(Directory.GetCurrentDirectory())          
               .UseStartup<Startup>()
               .Build();
    }
}
