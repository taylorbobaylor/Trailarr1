using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Common.TPL;
using NzbDrone.Core.Datastore.Events;
using NzbDrone.Core.Download.Pending;
using NzbDrone.Core.Download.TrackedDownloads;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Queue;
using NzbDrone.SignalR;
using Radarr.Http;
using Radarr.Http.REST;

namespace Radarr.Api.V3.Queue
{
    [V3ApiController("queue/status")]
    public class QueueStatusController : RestControllerWithSignalR<QueueStatusResource, NzbDrone.Core.Queue.Queue>,
                               IHandle<QueueUpdatedEvent>
    {
        private readonly IQueueService _queueService;
        private readonly Debouncer _broadcastDebounce;

        public QueueStatusController(IBroadcastSignalRMessage broadcastSignalRMessage, IQueueService queueService)
            : base(broadcastSignalRMessage)
        {
            _queueService = queueService;

            _broadcastDebounce = new Debouncer(BroadcastChange, TimeSpan.FromSeconds(5));
        }

        [NonAction]
        public override ActionResult<QueueStatusResource> GetResourceByIdWithErrorHandler(int id)
        {
            return base.GetResourceByIdWithErrorHandler(id);
        }

        protected override QueueStatusResource GetResourceById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public QueueStatusResource GetQueueStatus()
        {
            _broadcastDebounce.Pause();

            var queue = _queueService.GetQueue();

            var resource = new QueueStatusResource
            {
                TotalCount = queue.Count,
                Count = queue.Count(q => q.Movie != null),
                UnknownCount = queue.Count(q => q.Movie == null),
                Errors = false,
                Warnings = false,
                UnknownErrors = false,
                UnknownWarnings = false
            };

            _broadcastDebounce.Resume();

            return resource;
        }

        private void BroadcastChange()
        {
            BroadcastResourceChange(ModelAction.Updated, GetQueueStatus());
        }

        [NonAction]
        public void Handle(QueueUpdatedEvent message)
        {
            _broadcastDebounce.Execute();
        }

        // Removed pending releases handling - trailer only functionality
    }
}
