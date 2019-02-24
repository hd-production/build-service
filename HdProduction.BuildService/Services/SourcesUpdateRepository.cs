using System;
using System.Threading.Tasks;
using HdProduction.Npgsql.Orm;
using Microsoft.Extensions.Caching.Memory;

namespace HdProduction.BuildService.Services
{
    public interface ISourcesUpdateRepository
    {
        Task<DateTime> GetSourcesUpdateDateAsync();
    }

    public class SourcesUpdateRepository : ISourcesUpdateRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IDatabaseConnector _databaseConnector;

        public SourcesUpdateRepository(IDatabaseConnector databaseConnector, IMemoryCache cache)
        {
            _databaseConnector = databaseConnector;
            _cache = cache;
        }

        public async Task<DateTime> GetSourcesUpdateDateAsync()
        {
            using (var cx = _databaseConnector.NewDataContext())
            {
                return await _cache.GetOrCreateAsync("sources_update",
                    async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                        return await cx.Sql("SELECT date FROM sources_update").ReadAsync<DateTime>();
                    });
            }
        }
    }
}