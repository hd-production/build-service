using System;
using HdProduction.App.Common;
using HdProduction.BuildService.MessageQueue;
using HdProduction.BuildService.Services;
using HdProduction.MessageQueue.RabbitMq;
using HdProduction.MessageQueue.RabbitMq.Events.AppBuilds;
using HdProduction.MessageQueue.RabbitMq.Stubs;
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
 
      services.AddSingleton<IDatabaseConnector>(new DatabaseConnector(Configuration.GetConnectionString("Db")));
      services.AddScoped<IHelpdeskBuildService, HelpdeskBuildService>(
        c => new HelpdeskBuildService(Configuration.GetValue<string>("Uris:HelpdeskHostSources")));
      services.AddScoped<IContentServiceClient, ContentServiceClient>(
        c => new ContentServiceClient(Configuration.GetValue<string>("Uris:ContentStorage")));
      services.AddScoped<ISourcesUpdateRepository, SourcesUpdateRepository>();
      services.AddScoped<IBuildsRepository, BuildsRepository>();

      AddMessageQueue(services, Configuration.GetSection("MessageQueue"));
    }
    private static void AddMessageQueue(IServiceCollection services, IConfigurationSection mqConfigurationSection)
    {
      if (mqConfigurationSection.GetValue<bool>("Enabled"))
      {
        services.AddSingleton<IRabbitMqConnection>(
          new RabbitMqConnection(mqConfigurationSection.GetValue<string>("Uri"), "hd_production"));
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>(c => new RabbitMqConsumer(
          mqConfigurationSection.GetValue<string>("ConsumerQueue"), c.GetService<IServiceProvider>(), c.GetService<IRabbitMqConnection>()));
      }
      else
      {
        services.AddSingleton<IRabbitMqPublisher, FakeMqPublisher>();
        services.AddSingleton<IRabbitMqConsumer, FakeMqConsumer>();
      }

      services.AddTransient<IMessageHandler<RequiresSelfHostBuildingMessage>, ProjectRequiresSelfHostBuildMessageHandler>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.ResolveService<IRabbitMqConsumer>()
        .Subscribe<RequiresSelfHostBuildingMessage>()
        .StartConsuming();

      app.Run(async context =>
      {
        await context.Response.WriteAsync("HdProduction.BuildService is running");
      });
    }
  }
}