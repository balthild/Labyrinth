import React from 'react';
import fs from 'fs-extra';
import { connect } from 'react-redux';

import './Profile.scss';
import { ClashConfig } from '@/types/ClashConfig';
import { Config } from '@/types/Config';
import { configDirPath, getConfigFilePath } from '@/util';
import PageTitle from '@/renderer/components/PageTitle';
import { Action, ActionTypes, GlobalState, ServiceStatus } from '@/renderer/store';
import { Dispatch } from 'redux';
import { getControllerUrl } from '@/renderer/util';

type ProfileProps = {
    appConfig: Config;
    updateAppConfig(appConfig: Config): void;
};

type ProfileState = {
    clashConfigFiles: string[];
};

class Profile extends React.Component<ProfileProps, ProfileState> {
    state = {
        clashConfigFiles: [],
    };

    setActiveConfig = async (e: React.MouseEvent<HTMLLIElement>) => {
        const filename = e.currentTarget.dataset.filename!!;
        const path = getConfigFilePath(filename);

        const configUrl = getControllerUrl('/configs');
        await fetch(configUrl, {
            method: 'PUT',
            body: JSON.stringify({ path }),
        });

        this.props.updateAppConfig({
            ...this.props.appConfig,
            configFile: filename,
        });
    };

    async loadConfigFiles() {
        const clashConfigFiles = (await fs.readdir(configDirPath))
            .filter(name => name.endsWith('yml') || name.endsWith('.yaml'));

        this.setState({ clashConfigFiles });
    }

    componentDidMount() {
        this.loadConfigFiles();
    }

    render() {
        return (
            <div className="profile">
                <PageTitle>Profile</PageTitle>

                <ul className="clash-configs">
                    {this.state.clashConfigFiles.map((name) => (
                        <li key={name} data-filename={name} onClick={this.setActiveConfig}>
                            <div className="name">{name}</div>
                            <div className="description">gasd</div>

                            {this.props.appConfig.configFile === name && (
                                <div className="current-sign">
                                    <i className="ion ion-ios-checkmark-circle" />
                                </div>
                            )}
                        </li>
                    ))}
                </ul>
            </div>
        );
    }
}

const mapStateToProps = (state: GlobalState) => ({
    config: state.config,
    appConfig: state.appConfig,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({
    updateAppConfig(appConfig: Config) {
        dispatch({
            type: ActionTypes.UPDATE_APP_CONFIG,
            appConfig,
        });
    },
});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Profile);
