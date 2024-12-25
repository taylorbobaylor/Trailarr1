using DryIoc;
using NzbDrone.Common.Composition;

namespace NzbDrone.Core.Trailers
{
    public static class ContainerBuilderExtensions
    {
        public static IContainer AddTrailerServices(this IContainer container)
        {
            container.Register<ITrailerDownloadService, TrailerDownloadService>();
            return container;
        }
    }
}
