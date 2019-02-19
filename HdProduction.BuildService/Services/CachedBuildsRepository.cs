using System;
using System.Threading.Tasks;
using HdProduction.Npgsql.Orm;

namespace HdProduction.BuildService.Services
{
  public interface ICachedBuildsRepository
  {
    Task<string> FindSelfHostBuildAsync(int buildConfig);
    Task UpsertSelfHostBuildAsync(string buildKey, int buildConfig);
  }

  public class CachedBuildsRepository : ICachedBuildsRepository
  {
    private readonly IDatabaseConnector _databaseConnector;

    public CachedBuildsRepository(IDatabaseConnector databaseConnector)
    {
      _databaseConnector = databaseConnector;
    }

    public async Task<string> FindSelfHostBuildAsync(int buildConfig)
    {
      using (var cx = _databaseConnector.NewDataContext())
      {
        return await cx.Sql("SELECT build_key FROM cached_builds WHERE date > @ExpirationDate AND self_host_config = @BuildConfig")
          .Set("ExpirationDate", DateTime.UtcNow.Subtract(TimeSpan.FromDays(5)))
          .Set("BuildConfig", buildConfig)
          .ReadAsync<string>();
      }
    }

    public async Task UpsertSelfHostBuildAsync(string buildKey, int buildConfig)
    {
      using (var cx = _databaseConnector.NewDataContext())
      {
        await cx.Sql(@"INSERT INTO cached_builds VALUES (@BuildKey, @Date, @BuildConfig)")
          .Set("BuildKey", buildKey)
          .Set("Date", DateTime.UtcNow)
          .Set("BuildConfig", buildConfig)
          .ExecuteAsync();
      }
    }
  }
}