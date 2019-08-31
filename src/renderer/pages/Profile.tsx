import React from 'react';
import fs from 'fs-extra';
import { connect } from 'react-redux';

import './Profile.scss';
import { Config } from '@/types/Config';
import { configDirPath, getConfigFilePath } from '@/util';
import PageTitle from '@/renderer/components/PageTitle';
import { Action, ActionTypes, GlobalState } from '@/renderer/store';
import { Dispatch } from 'redux';
import { getControllerUrl } from '@/renderer/util';
import { writeAppConfig } from '@/renderer/config';

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

    setActiveConfig = async (e: React.MouseEvent<HTMLDivElement>) => {
        const filename = e.currentTarget.dataset.filename!!;
        const path = getConfigFilePath(filename);

        if (!await fs.pathExists(path)) {
            await this.loadConfigFiles();
            return;
        }

        const configUrl = getControllerUrl('/configs');
        await fetch(configUrl, {
            method: 'PUT',
            body: JSON.stringify({ path }),
        });

        const appConfig = {
            ...this.props.appConfig,
            configFile: filename,
        };

        this.props.updateAppConfig(appConfig);

        await Promise.all([
            writeAppConfig(appConfig),
            this.loadConfigFiles(),
        ]);
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
        const current = this.props.appConfig.configFile;

        return (
            <div className="profile content-inner">
                <PageTitle>Profile</PageTitle>

                <ul className="clash-configs">
                    {this.state.clashConfigFiles.map((name) => (
                        <li key={name} className={name === current ? 'current' : ''}>
                            <div className="name">{name}</div>
                            <div className="description">gasd</div>

                            <div className="active-sign" data-filename={name} onClick={this.setActiveConfig}>
                                <i className={`ion ${activeSignIconClass(name, current)}`} />
                            </div>
                        </li>
                    ))}
                </ul>
            </div>
        );
    }
}

const activeSignIconClass = (config: string, currentConfig: string) =>
    config === currentConfig ?
        'ion-ios-checkmark-circle' :
        'ion-ios-checkmark-circle-outline';

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
