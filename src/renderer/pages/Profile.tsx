import React from 'react';
import fs from 'fs-extra';
import { connect } from 'react-redux';
import Prism from 'prismjs';
import 'prismjs/components/prism-yaml';

import './Profile.scss';
import { Config } from '@/types/Config';
import { configDirPath, getConfigFilePath } from '@/util';
import PageTitle from '@/renderer/components/PageTitle';
import { Action, ActionTypes, GlobalState } from '@/renderer/store';
import { Dispatch } from 'redux';
import { getControllerUrl } from '@/renderer/util';
import { getClashConfig, writeAppConfig } from '@/renderer/config';
import Detail from '@/renderer/components/Detail';

type ProfileProps = {
    appConfig: Config;
    updateAppConfig(appConfig: Config): void;
};

type ProfileState = {
    clashConfigFiles: string[];
    showDetail: boolean;
    detailName: string;
    detailContent: string;
};

class Profile extends React.Component<ProfileProps, ProfileState> {
    state = {
        clashConfigFiles: [],
        showDetail: false,
        detailName: '',
        detailContent: '',
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

    showDetail = async (e: React.MouseEvent<HTMLLIElement>) => {
        const filename = e.currentTarget.dataset.filename!!;
        const path = getConfigFilePath(filename);
        const data = await fs.readFile(path);
        const content = Prism.highlight(data.toString(), Prism.languages.yaml, 'yaml');

        this.setState({
            showDetail: true,
            detailName: filename,
            detailContent: content,
        });
    };

    hideDetail = async (e: React.MouseEvent) => {
        this.setState({
            showDetail: false,
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
        const current = this.props.appConfig.configFile;

        const { showDetail, detailName, detailContent } = this.state;

        return (
            <>
                <div className="profile content-inner">
                    <PageTitle>Profile</PageTitle>

                    <ul className="clash-configs">
                        {this.state.clashConfigFiles.map((name) => (
                            <li
                                key={name}
                                className={name === current ? 'current' : ''}
                                data-filename={name}
                                onClick={this.showDetail}
                            >
                                <div className="name">{name}</div>
                                <div className="description">{/* TODO */}</div>

                                <div className="active-sign" data-filename={name} onClick={this.setActiveConfig}>
                                    <i className={`ion ${activeSignIconClass(name, current)}`} />
                                </div>
                            </li>
                        ))}
                    </ul>
                </div>

                <Detail isOpened={showDetail} title={detailName} onCloseButtonClick={this.hideDetail}>
                    <pre className="config-code" dangerouslySetInnerHTML={{ __html: detailContent }} />
                </Detail>
            </>
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
