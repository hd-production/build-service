using HdProduction.App.Common;
using HdProduction.BuildService.MessageQueue;
using HdProduction.BuildService.MessageQueue.Events;
using HdProduction.BuildService.Services;
using HdProduction.Npgsql.Orm;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HdProduction.BuildService
{
  public class Startup
  {
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
    
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMemoryCache();
      services.AddMessageQueue<ProjectRequiresSelfHostBuildEventHandler>(Configuration.GetSection(""));
      services.AddSingleton<IDatabaseConnector>(new DatabaseConnector(Configuration.GetConnectionString("Db")));
      services.AddScoped<IHelpdeskBuildService, HelpdeskBuildService>(
        c => new HelpdeskBuildService(Configuration.GetValue<string>("Uris:HelpdeskHostSources")));
      services.AddScoped<IContentServiceClient, ContentServiceClient>(
        c => new ContentServiceClient(Configuration.GetValue<string>("Uris:ContentStorage")));
      services.AddScoped<ISourcesUpdateRepository, SourcesUpdateRepository>();
      services.AddScoped<IBuildsRepository, BuildsRepository>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.SetEventConsumer()
        .Subscribe<ProjectRequiresSelfHostBuildingEvent>()
        .StartConsuming();

      app.Run(async context =>
      {
        await context.Response.WriteAsync("HdProduction.BuildService is running");
      });
    }
  }
}