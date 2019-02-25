using System.Threading.Tasks;
using HdProduction.App.Common.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace HdProduction.BuildService
{
  public class Program
  {
    public static Task Main(string[] args)
    {
      return CreateWebHostBuilder(args).Build().RunAsync();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
        .UseUrls("http://0.0.0.0:5002")
        .ConfigureLogging((hostingContext, logging) => logging.AddLog4Net())
        .UseStartup<Startup>();
  }
}