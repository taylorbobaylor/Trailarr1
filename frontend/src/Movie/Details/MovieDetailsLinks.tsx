import React from 'react';
import Label from 'Components/Label';
import Link from 'Components/Link/Link';
import { kinds, sizes } from 'Helpers/Props';
import Movie from 'Movie/Movie';
import translate from 'Utilities/String/translate';
import styles from './MovieDetailsLinks.css';

type MovieDetailsLinksProps = Pick<
  Movie,
  'tmdbId' | 'youTubeTrailerId' | 'localTrailerPath'
>;

function MovieDetailsLinks(props: MovieDetailsLinksProps) {
  const { tmdbId, youTubeTrailerId, localTrailerPath } = props;

  return (
    <div className={styles.links}>
      <Link
        className={styles.link}
        to={`https://www.themoviedb.org/movie/${tmdbId}`}
      >
        <Label
          className={styles.linkLabel}
          kind={kinds.INFO}
          size={sizes.LARGE}
        >
          {translate('TMDb')}
        </Label>
      </Link>

      {localTrailerPath ? (
        <Label
          className={styles.linkLabel}
          kind={kinds.SUCCESS}
          size={sizes.LARGE}
        >
          {translate('TrailerDownloaded')}
        </Label>
      ) : youTubeTrailerId ? (
        <Link
          className={styles.link}
          to={`https://www.youtube.com/watch?v=${youTubeTrailerId}`}
        >
          <Label
            className={styles.linkLabel}
            kind={kinds.DANGER}
            size={sizes.LARGE}
          >
            {translate('Trailer')}
          </Label>
        </Link>
      ) : null}
    </div>
  );
}

export default MovieDetailsLinks;
