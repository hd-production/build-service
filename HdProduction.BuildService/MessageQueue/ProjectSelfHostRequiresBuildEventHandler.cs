using System;
using System.Reflection;
using System.Threading.Tasks;
using HdProduction.BuildService.MessageQueue.Events;
using HdProduction.BuildService.Services;
using HdProduction.BuildService.Synchronization;
using HdProduction.MessageQueue.RabbitMq;
using log4net;

namespace HdProduction.BuildService.MessageQueue
{
    public class ProjectRequiresSelfHostBuildEventHandler : IEventHandler<ProjectRequiresSelfHostBuildingEvent>
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRabbitMqPublisher _publisher;
        private readonly IBuildsRepository _buildsRepository;
        private readonly ISourcesUpdateRepository _sourcesUpdateRepository;
        private readonly IHelpdeskBuildService _helpdeskBuildService;
        private readonly IContentServiceClient _contentServiceClient;

        public ProjectRequiresSelfHostBuildEventHandler(IRabbitMqPublisher publisher, 
            IBuildsRepository buildsRepository, ISourcesUpdateRepository sourcesUpdateRepository, 
            IHelpdeskBuildService helpdeskBuildService, IContentServiceClient contentServiceClient)
        {
            _publisher = publisher;
            _buildsRepository = buildsRepository;
            _sourcesUpdateRepository = sourcesUpdateRepository;
            _helpdeskBuildService = helpdeskBuildService;
            _contentServiceClient = contentServiceClient;
        }

        public async Task HandleAsync(ProjectRequiresSelfHostBuildingEvent ev)
        {
            try
            {
                var buildKey = await ProcessProjectBuildRequestAsync(ev.SelfHostConfiguration);
                await _publisher.PublishAsync(new SelfHostBuiltEvent
                {
                    ProjectId = ev.ProjectId,
                    DownloadLink = _contentServiceClient.GetDownloadLink(buildKey)
                });
            }
            catch (Exception e)
            {
                Logger.Error($"Exception while handling self host build request projectId: {ev.ProjectId}, config: {ev.SelfHostConfiguration}", e);
                await _publisher.PublishAsync(new SelfHostBuildingFailedEvent{ProjectId = ev.ProjectId, Exception = e.ToString()});
            }
        }

        private async Task<string> ProcessProjectBuildRequestAsync(int selfHostConfiguration)
        {
            using (new AppBuildLocker(selfHostConfiguration))
            {
                var sourcesUpdate = await _sourcesUpdateRepository.GetSourcesUpdateDateAsync();
                var existingBuildKey =
                    await _buildsRepository.FindSelfHostBuildAsync(selfHostConfiguration, sourcesUpdate);
                if (existingBuildKey != null)
                {
                    return existingBuildKey;
                }

                var buildArchivePath = _helpdeskBuildService.BuildApp(selfHostConfiguration);
                var fileKey = await _contentServiceClient.UploadFileAsync(buildArchivePath);
                await _buildsRepository.UpsertSelfHostBuildAsync(fileKey, selfHostConfiguration);
                return fileKey;
            }
        }
    }
}