using System.Collections.Generic;
using System.Linq;
using NzbDrone.Common.Disk;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Datastore.Events;
using NzbDrone.Core.Localization;
using NzbDrone.Core.Movies.Collections;
using NzbDrone.Core.Movies.Events;
using NzbDrone.Core.RootFolders;

namespace NzbDrone.Core.HealthCheck.Checks;
    [CheckOn(typeof(CollectionRefreshCompleteEvent))]
    [CheckOn(typeof(CollectionEditedEvent))]
    [CheckOn(typeof(CollectionDeletedEvent))]
    [CheckOn(typeof(ModelEvent<RootFolder>))]
    public class MovieCollectionRootFolderCheck : HealthCheckBase
    {
        private readonly IMovieCollectionService _collectionService;
        private readonly IDiskProvider _diskProvider;
        private readonly IRootFolderService _rootFolderService;

        public MovieCollectionRootFolderCheck(IMovieCollectionService collectionService, IDiskProvider diskProvider, IRootFolderService rootFolderService, ILocalizationService localizationService)
            : base(localizationService)
        {
            _collectionService = collectionService;
            _diskProvider = diskProvider;
            _rootFolderService = rootFolderService;
        }

        public override HealthCheck Check()
        {
            var collections = _collectionService.GetAllCollections();
            var rootFolders = _rootFolderService.All();

            var missingRootFolders = new Dictionary<string, List<MovieCollection>>();

            foreach (var collection in collections)
            {
                var rootFolderPath = collection.RootFolderPath;

                if (missingRootFolders.ContainsKey(rootFolderPath))
                {
                    missingRootFolders[rootFolderPath].Add(collection);

                    continue;
                }

                if (rootFolderPath.IsNullOrWhiteSpace() ||
                    !rootFolderPath.IsPathValid(PathValidationType.CurrentOs) ||
                    !rootFolders.Any(r => r.Path.PathEquals(rootFolderPath)) ||
                    !_diskProvider.FolderExists(rootFolderPath))
                {
                    missingRootFolders.Add(rootFolderPath, new List<MovieCollection> { collection });
                }
            }

            if (missingRootFolders.Any())
            {
                if (missingRootFolders.Count == 1)
                {
                    var missingRootFolder = missingRootFolders.First();

                    return new HealthCheck(GetType(),
                        HealthCheckResult.Error,
                        _localizationService.GetLocalizedString("MovieCollectionRootFolderMissingRootHealthCheckMessage", new Dictionary<string, object>
                        {
                            { "rootFolderInfo", FormatRootFolder(missingRootFolder.Key, missingRootFolder.Value) }
                        }),
                        "#movie-collection-missing-root-folder");
                }

                return new HealthCheck(GetType(),
                    HealthCheckResult.Error,
                    _localizationService.GetLocalizedString("MovieCollectionFolderMultipleMissingRootsHealthCheckMessage", new Dictionary<string, object>
                    {
                        { "rootFoldersInfo", string.Join(" | ", missingRootFolders.Select(m => FormatRootFolder(m.Key, m.Value))) }
                    }),
                    "#movie-collection-missing-root-folder");
            }

            return new HealthCheck(GetType());
        }

        private string FormatRootFolder(string rootFolderPath, List<MovieCollection> collections)
        {
            return $"{rootFolderPath} ({string.Join(", ", collections.Select(c => c.Title))})";
        }
    }
