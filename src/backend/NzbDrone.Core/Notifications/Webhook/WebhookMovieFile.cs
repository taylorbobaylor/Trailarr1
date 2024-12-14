using System;
using System.Collections.Generic;
using NzbDrone.Core.Languages;
using NzbDrone.Core.MediaFiles;

namespace NzbDrone.Core.Notifications.Webhook;
    public class WebhookMovieFile
    {
        public WebhookMovieFile()
        {
        }

        public WebhookMovieFile(MovieFile movieFile)
        {
            Id = movieFile.Id;
            RelativePath = movieFile.RelativePath;
            Path = System.IO.Path.Combine(movieFile.Movie.Path, movieFile.RelativePath);
            Quality = movieFile.Quality.Quality.Name;
            QualityVersion = movieFile.Quality.Revision.Version;
            ReleaseGroup = movieFile.ReleaseGroup;
            SceneName = movieFile.SceneName;
            IndexerFlags = movieFile.IndexerFlags.ToString();
            Size = movieFile.Size;
            DateAdded = movieFile.DateAdded;
            Languages = movieFile.Languages;

            if (movieFile.MediaInfo != null)
            {
                MediaInfo = new WebhookMovieFileMediaInfo(movieFile);
            }
        }

        public int Id { get; set; }
        public string RelativePath { get; set; }
        public string Path { get; set; }
        public string Quality { get; set; }
        public int QualityVersion { get; set; }
        public string ReleaseGroup { get; set; }
        public string SceneName { get; set; }
        public string IndexerFlags { get; set; }
        public long Size { get; set; }
        public DateTime DateAdded { get; set; }
        public List<Language> Languages { get; set; }
        public WebhookMovieFileMediaInfo MediaInfo { get; set; }
        public string SourcePath { get; set; }
        public string RecycleBinPath { get; set; }
    }
