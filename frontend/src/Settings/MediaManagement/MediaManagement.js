import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Alert from 'Components/Alert';
import FieldSet from 'Components/FieldSet';
import Form from 'Components/Form/Form';
import FormGroup from 'Components/Form/FormGroup';
import FormInputGroup from 'Components/Form/FormInputGroup';
import FormLabel from 'Components/Form/FormLabel';
import LoadingIndicator from 'Components/Loading/LoadingIndicator';
import PageContent from 'Components/Page/PageContent';
import PageContentBody from 'Components/Page/PageContentBody';
import { inputTypes, kinds, sizes } from 'Helpers/Props';
import RootFolders from 'RootFolder/RootFolders';
import SettingsToolbarConnector from 'Settings/SettingsToolbarConnector';
import translate from 'Utilities/String/translate';
import AddRootFolder from './RootFolder/AddRootFolder';



class MediaManagement extends Component {

  //
  // Render

  render() {
    const {
      advancedSettings,
      isFetching,
      error,
      settings,
      hasSettings,
      isWindows,
      onInputChange,
      onSavePress,
      ...otherProps
    } = this.props;

    return (
      <PageContent title={translate('MediaManagementSettings')}>
        <SettingsToolbarConnector
          advancedSettings={advancedSettings}
          {...otherProps}
          onSavePress={onSavePress}
        />

        <PageContentBody>
          {
            isFetching ?
              <FieldSet legend={translate('TrailerSettings')}>
                <LoadingIndicator />
              </FieldSet> : null
          }

          {
            !isFetching && error ?
              <FieldSet legend={translate('TrailerSettings')}>
                <Alert kind={kinds.DANGER}>
                  {translate('MediaManagementSettingsLoadError')}
                </Alert>
              </FieldSet> : null
          }

          {
            hasSettings && !isFetching && !error ?
              <Form
                id="mediaManagementSettings"
                {...otherProps}
              >
                <FieldSet
                  legend={translate('TrailerSettings')}
                >
                  <FormGroup size={sizes.MEDIUM}>
                    <FormLabel>{translate('AutoDownloadTrailer')}</FormLabel>

                    <FormInputGroup
                      type={inputTypes.CHECK}
                      name="autoDownloadTrailer"
                      helpText={translate('AutoDownloadTrailerHelpText')}
                      onChange={onInputChange}
                      {...settings.autoDownloadTrailer}
                    />
                  </FormGroup>

                  <FormGroup size={sizes.MEDIUM}>
                    <FormLabel>{translate('TrailerFolder')}</FormLabel>

                    <FormInputGroup
                      type={inputTypes.PATH}
                      name="trailerFolder"
                      helpText={translate('TrailerFolderHelpText')}
                      onChange={onInputChange}
                      {...settings.trailerFolder}
                    />
                  </FormGroup>
                </FieldSet>

                <FieldSet legend={translate('RootFolders')}>
                  <RootFolders />
                  <AddRootFolder />
                </FieldSet>
              </Form> : null
          }
        </PageContentBody>
      </PageContent>
    );
  }

}

MediaManagement.propTypes = {
  advancedSettings: PropTypes.bool.isRequired,
  isFetching: PropTypes.bool.isRequired,
  error: PropTypes.object,
  settings: PropTypes.object.isRequired,
  hasSettings: PropTypes.bool.isRequired,
  isWindows: PropTypes.bool.isRequired,
  onSavePress: PropTypes.func.isRequired,
  onInputChange: PropTypes.func.isRequired
};

export default MediaManagement;
