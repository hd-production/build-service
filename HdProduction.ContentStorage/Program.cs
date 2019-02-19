using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace HdProduction.ContentStorage
{
  public class Program
  {
    public static Task Main(string[] args)
    {
      return CreateWebHostBuilder(args).Build().RunAsync();
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
        .UseUrls("http://0.0.0.0:5003")
        .UseStartup<Startup>();
  }
}