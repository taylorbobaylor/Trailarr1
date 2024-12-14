using System.Collections.Generic;
using NzbDrone.Core.ImportLists;
using NzbDrone.Core.Movies;

namespace Radarr.Api.V3.ImportLists;
    public class ImportListBulkResource : ProviderBulkResource<ImportListBulkResource>
    {
        public bool? Enabled { get; set; }
        public bool? EnableAuto { get; set; }
        public string RootFolderPath { get; set; }
        public int? QualityProfileId { get; set; }
        public MovieStatusType? MinimumAvailability { get; set; }
    }

    public class ImportListBulkResourceMapper : ProviderBulkResourceMapper<ImportListBulkResource, ImportListDefinition>
    {
        public override List<ImportListDefinition> UpdateModel(ImportListBulkResource resource, List<ImportListDefinition> existingDefinitions)
        {
            if (resource == null)
            {
                return new List<ImportListDefinition>();
            }

            existingDefinitions.ForEach(existing =>
            {
                existing.Enabled = resource.Enabled ?? existing.Enabled;
                existing.EnableAuto = resource.EnableAuto ?? existing.EnableAuto;
                existing.RootFolderPath = resource.RootFolderPath ?? existing.RootFolderPath;
                existing.QualityProfileId = resource.QualityProfileId ?? existing.QualityProfileId;
                existing.MinimumAvailability = resource.MinimumAvailability ?? existing.MinimumAvailability;
            });

            return existingDefinitions;
        }
    }
