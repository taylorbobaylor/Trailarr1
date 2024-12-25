using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(250)]
    public class add_local_trailer_path : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("MovieMetadata")
                .AddColumn("LocalTrailerPath").AsString().Nullable();
        }
    }
}
