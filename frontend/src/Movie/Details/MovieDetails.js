import PropTypes from 'prop-types';
import React, { Component } from 'react';
import TextTruncate from 'react-text-truncate';
import Alert from 'Components/Alert';
import FieldSet from 'Components/FieldSet';
import Icon from 'Components/Icon';
import InfoLabel from 'Components/InfoLabel';
import IconButton from 'Components/Link/IconButton';
import Marquee from 'Components/Marquee';
import Measure from 'Components/Measure';
import MonitorToggleButton from 'Components/MonitorToggleButton';
import PageContent from 'Components/Page/PageContent';
import PageContentBody from 'Components/Page/PageContentBody';
import PageToolbar from 'Components/Page/Toolbar/PageToolbar';
import PageToolbarButton from 'Components/Page/Toolbar/PageToolbarButton';
import PageToolbarSection from 'Components/Page/Toolbar/PageToolbarSection';
import PageToolbarSeparator from 'Components/Page/Toolbar/PageToolbarSeparator';
import { icons, kinds, sizes, tooltipPositions } from 'Helpers/Props';
import DeleteMovieModal from 'Movie/Delete/DeleteMovieModal';
import EditMovieModalConnector from 'Movie/Edit/EditMovieModalConnector';
import getMovieStatusDetails from 'Movie/getMovieStatusDetails';
import MoviePoster from 'Movie/MoviePoster';
import Label from 'Components/Label';
import fonts from 'Styles/Variables/fonts';
import * as keyCodes from 'Utilities/Constants/keyCodes';
import formatRuntime from 'Utilities/Date/formatRuntime';
import formatBytes from 'Utilities/Number/formatBytes';
import translate from 'Utilities/String/translate';
import MovieCastPosters from './Credits/Cast/MovieCastPosters';
import MovieCrewPosters from './Credits/Crew/MovieCrewPosters';
import MovieDetailsLinks from './MovieDetailsLinks';
import MovieReleaseDates from './MovieReleaseDates';
import MovieStatusLabel from './MovieStatusLabel';
import MovieTagsConnector from './MovieTagsConnector';
import MovieTitlesTable from './Titles/MovieTitlesTable';
import styles from './MovieDetails.css';

const defaultFontSize = parseInt(fonts.defaultFontSize);
const lineHeight = parseFloat(fonts.lineHeight);

function getFanartUrl(images) {
  const image = images.find((img) => img.coverType === 'fanart');
  return image?.url ?? image?.remoteUrl;
}

class MovieDetails extends Component {

  //
  // Lifecycle

  constructor(props, context) {
    super(props, context);

    this.state = {
      overviewHeight: 0,
      titleWidth: 0
    };
  }

  componentDidMount() {
    window.addEventListener('touchstart', this.onTouchStart);
    window.addEventListener('touchend', this.onTouchEnd);
    window.addEventListener('touchcancel', this.onTouchCancel);
    window.addEventListener('touchmove', this.onTouchMove);
    window.addEventListener('keyup', this.onKeyUp);
  }

  componentWillUnmount() {
    window.removeEventListener('touchstart', this.onTouchStart);
    window.removeEventListener('touchend', this.onTouchEnd);
    window.removeEventListener('touchcancel', this.onTouchCancel);
    window.removeEventListener('touchmove', this.onTouchMove);
    window.removeEventListener('keyup', this.onKeyUp);
  }

  //
  // Listeners

  onEditMoviePress = () => {
    this.setState({ isEditMovieModalOpen: true });
  };

  onEditMovieModalClose = () => {
    this.setState({ isEditMovieModalOpen: false });
  };

  onDeleteMoviePress = () => {
    this.setState({
      isEditMovieModalOpen: false,
      isDeleteMovieModalOpen: true
    });
  };

  onDeleteMovieModalClose = () => {
    this.setState({ isDeleteMovieModalOpen: false });
  };

  onMeasure = ({ height }) => {
    this.setState({ overviewHeight: height });
  };

  onTitleMeasure = ({ width }) => {
    this.setState({ titleWidth: width });
  };

  onKeyUp = (event) => {
    if (event.composedPath && event.composedPath().length === 4) {
      if (event.keyCode === keyCodes.LEFT_ARROW) {
        this.props.onGoToMovie(this.props.previousMovie.titleSlug);
      }
      if (event.keyCode === keyCodes.RIGHT_ARROW) {
        this.props.onGoToMovie(this.props.nextMovie.titleSlug);
      }
    }
  };

  onTouchStart = (event) => {
    const touches = event.touches;
    const touchStart = touches[0].pageX;
    const touchY = touches[0].pageY;

    // Only change when swipe is on header, we need horizontal scroll on tables
    if (touchY > 470) {
      return;
    }

    if (touches.length !== 1) {
      return;
    }

    if (
      touchStart < 50 ||
      this.props.isSidebarVisible ||
      this.state.isOrganizeModalOpen ||
      this.state.isEditMovieModalOpen ||
      this.state.isDeleteMovieModalOpen ||
      this.state.isInteractiveImportModalOpen ||
      this.state.isInteractiveSearchModalOpen ||
      this.state.isMovieHistoryModalOpen
    ) {
      return;
    }

    this._touchStart = touchStart;
  };

  onTouchEnd = (event) => {
    const touches = event.changedTouches;
    const currentTouch = touches[0].pageX;

    if (!this._touchStart) {
      return;
    }

    if (currentTouch > this._touchStart && currentTouch - this._touchStart > 100) {
      this.props.onGoToMovie(this.props.previousMovie.titleSlug);
    } else if (currentTouch < this._touchStart && this._touchStart - currentTouch > 100) {
      this.props.onGoToMovie(this.props.nextMovie.titleSlug);
    }

    this._touchStart = null;
  };

  onTouchCancel = (event) => {
    this._touchStart = null;
  };

  onTouchMove = (event) => {
    if (!this._touchStart) {
      return;
    }
  };

  //
  // Render

  render() {
    const {
      id,
      tmdbId,
      imdbId,
      title,
      originalTitle,
      year,
      inCinemas,
      physicalRelease,
      digitalRelease,
      runtime,
      certification,
      ratings,
      path,
      statistics,
      qualityProfileId,
      monitored,
      studio,
      originalLanguage,
      genres,
      collection,
      overview,
      status,
      youTubeTrailerId,
      isAvailable,
      images,
      tags,
      isSaving,
      isRefreshing,
      isSearching,
      isFetching,
      isSmallScreen,
      movieFilesError,
      movieCreditsError,
      extraFilesError,
      hasMovieFiles,
      previousMovie,
      nextMovie,
      onMonitorTogglePress,
      onRefreshPress,
      onSearchPress,
      queueItem,
      movieRuntimeFormat
    } = this.props;

    const {
      sizeOnDisk = 0
    } = statistics;

    const {
      isOrganizeModalOpen,
      isEditMovieModalOpen,
      isDeleteMovieModalOpen,
      isInteractiveImportModalOpen,
      isInteractiveSearchModalOpen,
      isMovieHistoryModalOpen,
      overviewHeight,
      titleWidth
    } = this.state;

    const statusDetails = getMovieStatusDetails(status);

    const fanartUrl = getFanartUrl(images);
    const marqueeWidth = isSmallScreen ? titleWidth : (titleWidth - 150);

    const titleWithYear = `${title}${year > 0 ? ` (${year})` : ''}`;

    return (
      <PageContent title={titleWithYear}>
        <PageToolbar>
          <PageToolbarSection>
            <PageToolbarButton
              label={translate('RefreshAndScan')}
              iconName={icons.REFRESH}
              spinningName={icons.REFRESH}
              title={translate('RefreshInformationAndScanDisk')}
              isSpinning={isRefreshing}
              onPress={onRefreshPress}
            />

            <PageToolbarSeparator />

            <PageToolbarButton
              label={translate('Edit')}
              iconName={icons.EDIT}
              onPress={this.onEditMoviePress}
            />

            <PageToolbarButton
              label={translate('Delete')}
              iconName={icons.DELETE}
              onPress={this.onDeleteMoviePress}
            />
          </PageToolbarSection>
        </PageToolbar>

        <PageContentBody innerClassName={styles.innerContentBody}>
          <div className={styles.header}>
            <div
              className={styles.backdrop}
              style={
                fanartUrl ?
                  { backgroundImage: `url(${fanartUrl})` } :
                  null
              }
            >
              <div className={styles.backdropOverlay} />
            </div>

            <div className={styles.headerContent}>
              <MoviePoster
                className={styles.poster}
                images={images}
                size={500}
                lazy={false}
              />

              <div className={styles.info}>
                <Measure onMeasure={this.onTitleMeasure}>
                  <div className={styles.titleRow}>
                    <div className={styles.titleContainer}>
                      <div className={styles.toggleMonitoredContainer}>
                        <MonitorToggleButton
                          className={styles.monitorToggleButton}
                          monitored={monitored}
                          isSaving={isSaving}
                          size={40}
                          onPress={onMonitorTogglePress}
                        />
                      </div>

                      <div className={styles.title} style={{ width: marqueeWidth }}>
                        <Marquee text={title} title={originalTitle} />
                      </div>
                    </div>

                    <div className={styles.movieNavigationButtons}>
                      <IconButton
                        className={styles.movieNavigationButton}
                        name={icons.ARROW_LEFT}
                        size={30}
                        title={translate('GoToInterp', [previousMovie.title])}
                        to={`/movie/${previousMovie.titleSlug}`}
                      />

                      <IconButton
                        className={styles.movieNavigationButton}
                        name={icons.ARROW_RIGHT}
                        size={30}
                        title={translate('GoToInterp', [nextMovie.title])}
                        to={`/movie/${nextMovie.titleSlug}`}
                      />
                    </div>
                  </div>
                </Measure>

                <div className={styles.details}>
                  <div>
                    {
                      certification ?
                        <span className={styles.certification} title={translate('Certification')}>
                          {certification}
                        </span> :
                        null
                    }

                    <span className={styles.year}>
                      <Popover
                        anchor={
                          year > 0 ? (
                            year
                          ) : (
                            <Icon
                              name={icons.WARNING}
                              kind={kinds.WARNING}
                              size={20}
                            />
                          )
                        }
                        title={translate('ReleaseDates')}
                        body={
                          <MovieReleaseDates
                            tmdbId={tmdbId}
                            inCinemas={inCinemas}
                            digitalRelease={digitalRelease}
                            physicalRelease={physicalRelease}
                          />
                        }
                        position={tooltipPositions.BOTTOM}
                      />
                    </span>

                    {
                      runtime ?
                        <span className={styles.runtime} title={translate('Runtime')}>
                          {formatRuntime(runtime, movieRuntimeFormat)}
                        </span> :
                        null
                    }

                    {
                      <span className={styles.links}>
                        <Tooltip
                          anchor={
                            <Icon
                              name={icons.EXTERNAL_LINK}
                              size={20}
                            />
                          }
                          tooltip={
                            <MovieDetailsLinks
                              tmdbId={tmdbId}
                              imdbId={imdbId}
                              youTubeTrailerId={youTubeTrailerId}
                            />
                          }
                          position={tooltipPositions.BOTTOM}
                        />
                      </span>
                    }

                    {
                      !!tags.length &&
                        <span>
                          <Tooltip
                            anchor={
                              <Icon
                                name={icons.TAGS}
                                size={20}
                              />
                            }
                            tooltip={
                              <MovieTagsConnector movieId={id} />
                            }
                            position={tooltipPositions.BOTTOM}
                          />
                        </span>
                    }
                  </div>
                </div>

                <div className={styles.details}></div>

                <div className={styles.detailsLabels}>
                  <InfoLabel
                    className={styles.detailsInfoLabel}
                    name={translate('Path')}
                    size={sizes.LARGE}
                  >
                    <span className={styles.path}>
                      {path}
                    </span>
                  </InfoLabel>
                </div>

                <Measure onMeasure={this.onMeasure}>
                  <div className={styles.overview}>
                    <TextTruncate
                      line={Math.floor(overviewHeight / (defaultFontSize * lineHeight))}
                      text={overview}
                    />
                  </div>
                </Measure>
              </div>
            </div>
          </div>

          <div className={styles.contentContainer}>
            {
              !isFetching && movieFilesError ?
                <Alert kind={kinds.DANGER}>
                  {translate('LoadingMovieFilesFailed')}
                </Alert> :
                null
            }

            {
              !isFetching && movieCreditsError ?
                <Alert kind={kinds.DANGER}>
                  {translate('LoadingMovieCreditsFailed')}
                </Alert> :
                null
            }

            {
              !isFetching && extraFilesError ?
                <Alert kind={kinds.DANGER}>
                  {translate('LoadingMovieExtraFilesFailed')}
                </Alert> :
                null
            }

            <FieldSet legend={translate('Trailer')}>
              <div className={styles.trailerStatus}>
                {localTrailerPath ? (
                  <Label kind={kinds.SUCCESS}>
                    {translate('TrailerDownloaded')}
                  </Label>
                ) : youTubeTrailerId ? (
                  <Label kind={kinds.WARNING}>
                    {translate('TrailerAvailable')}
                  </Label>
                ) : (
                  <Label kind={kinds.DANGER}>
                    {translate('NoTrailerFound')}
                  </Label>
                )}
              </div>
            </FieldSet>
          </div>

          <EditMovieModalConnector
            isOpen={isEditMovieModalOpen}
            movieId={id}
            onModalClose={this.onEditMovieModalClose}
            onDeleteMoviePress={this.onDeleteMoviePress}
          />

          <DeleteMovieModal
            isOpen={isDeleteMovieModalOpen}
            movieId={id}
            onModalClose={this.onDeleteMovieModalClose}
            nextMovieRelativePath={`/movie/${nextMovie.titleSlug}`}
          />
        </PageContentBody>
      </PageContent>
    );
  }
}

MovieDetails.propTypes = {
  id: PropTypes.number.isRequired,
  title: PropTypes.string.isRequired,
  originalTitle: PropTypes.string,
  year: PropTypes.number.isRequired,
  path: PropTypes.string.isRequired,
  monitored: PropTypes.bool.isRequired,
  youTubeTrailerId: PropTypes.string,
  localTrailerPath: PropTypes.string,
  digitalRelease: PropTypes.string,
  overview: PropTypes.string.isRequired,
  images: PropTypes.arrayOf(PropTypes.object).isRequired,
  alternateTitles: PropTypes.arrayOf(PropTypes.string).isRequired,
  tags: PropTypes.arrayOf(PropTypes.number).isRequired,
  isSaving: PropTypes.bool.isRequired,
  isRefreshing: PropTypes.bool.isRequired,
  isSearching: PropTypes.bool.isRequired,
  isFetching: PropTypes.bool.isRequired,
  isPopulated: PropTypes.bool.isRequired,
  isSmallScreen: PropTypes.bool.isRequired,
  isSidebarVisible: PropTypes.bool.isRequired,
  localTrailerPath: PropTypes.string,
  hasMovieFiles: PropTypes.bool.isRequired,
  previousMovie: PropTypes.object.isRequired,
  nextMovie: PropTypes.object.isRequired,
  onMonitorTogglePress: PropTypes.func.isRequired,
  onRefreshPress: PropTypes.func.isRequired,
  onSearchPress: PropTypes.func.isRequired,
  onGoToMovie: PropTypes.func.isRequired,
  queueItem: PropTypes.object,
  movieRuntimeFormat: PropTypes.string.isRequired
};

MovieDetails.defaultProps = {
  genres: [],
  statistics: {},
  tags: [],
  isSaving: false
};

export default MovieDetails;
