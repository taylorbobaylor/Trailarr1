using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Profiles.Delay;
using Radarr.Http.REST;

namespace Radarr.Api.V3.Profiles.Delay
{
    public class DelayProfileResource : RestResource
    {
        public bool AutoDownloadTrailers { get; set; }
        public int Order { get; set; }
        public HashSet<int> Tags { get; set; }
    }

    public static class DelayProfileResourceMapper
    {
        public static DelayProfileResource ToResource(this DelayProfile model)
        {
            if (model == null)
            {
                return null;
            }

            return new DelayProfileResource
            {
                Id = model.Id,
                AutoDownloadTrailers = model.AutoDownloadTrailers,
                Order = model.Order,
                Tags = new HashSet<int>(model.Tags)
            };
        }

        public static DelayProfile ToModel(this DelayProfileResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            return new DelayProfile
            {
                Id = resource.Id,
                AutoDownloadTrailers = resource.AutoDownloadTrailers,
                Order = resource.Order,
                Tags = new HashSet<int>(resource.Tags)
            };
        }

        public static List<DelayProfileResource> ToResource(this IEnumerable<DelayProfile> models)
        {
            return models.Select(ToResource).ToList();
        }
    }
}
