import React from 'react';
import is from 'electron-is';
import fs from 'fs-extra';
import tar from 'tar-stream';
import zlib from 'zlib';
import http from 'http';
import { Reader as GeoLiteReader } from '@maxmind/geoip2-node';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import './App.scss';
import { configDirPath, getConfigFilePath } from '@/util';
import { ClashConfig } from '@/types/ClashConfig';
import { Config } from '@/types/Config';
import { ClashController } from '@/types/ClashController';
import { getControllerUrl, ipcSendWithId } from './util';
import { getAppConfig, getClashController } from './config';
import { Action, ActionTypes, GlobalState, ServiceStatus } from './store';
import Sidebar from './components/Sidebar';
import Loading from './components/Loading';
import OverlayMessage from './components/OverlayMessage';
import Progress from './components/Progress';
import Overview from './pages/Overview';
import Proxy from './pages/Proxy';
import Profile from './pages/Profile';

type AppProps = {
    ready: boolean;
    location: string;
    updateConfig(status: ServiceStatus, config: ClashConfig): void;
    updateAppConfig(appConfig: Config): void;
    serviceReady(controller: ClashController): void;
};

type AppState = {
    isWindowFocused: boolean;
    downloadingGeoLite: boolean;
    downloadingProgress: number;
};

// See: https://github.com/gabrielbull/react-desktop/blob/master/src/windowFocus.js
class App extends React.Component<AppProps, AppState> {
    state = {
        isWindowFocused: document.hasFocus(),
        downloadingGeoLite: false,
        downloadingProgress: 0,
    };

    windowFocus = () => {
        this.setState({ isWindowFocused: true });
    };

    windowBlur = () => {
        this.setState({ isWindowFocused: false });
    };

    getCurrentClashConfig = async () => {
        const configUrl = getControllerUrl('/configs');

        const config = await fetch(configUrl).then(r => r.json());

        this.props.updateConfig(ServiceStatus.Running, config);
    };

    async ensureGeoLite() {
        const path = getConfigFilePath('Country.mmdb');

        if (await fs.pathExists(path)) {
            try {
                await GeoLiteReader.open(path);
                return;
            } catch {
                await fs.unlink(path);
            }
        }

        this.setState({
            downloadingGeoLite: true,
        });

        const file = fs.createWriteStream(path);
        const extract = tar.extract()
            .on('entry', (header, stream, next) => {
                stream.on('data', (chunk) => {
                    if (header.name.endsWith('GeoLite2-Country.mmdb')) {
                        file.write(chunk);
                    }
                });

                stream.on('end', next);
                stream.resume();
            })
            .on('finish', function () {
                file.end();
            });

        const url = 'http://geolite.maxmind.com/download/geoip/database/GeoLite2-Country.tar.gz';
        await new Promise((resolve, reject) => {
            http.get(url, (response) => {
                const totalSize = parseInt(response.headers['content-length']!!, 10);
                let downloadedSize = 0;

                response.pipe(zlib.createGunzip()).pipe(extract);

                response.on('data', (chunk: Buffer) => {
                    downloadedSize += chunk.length;

                    this.setState({
                        downloadingProgress: downloadedSize / totalSize,
                    });
                });

                response.on('error', reject);
                response.on('end', resolve);
            });
        });

        this.setState({
            downloadingGeoLite: false,
        });
    }

    async initialize() {
        await fs.mkdirp(configDirPath);
        await this.ensureGeoLite();

        const config = await getAppConfig();
        const controller = await getClashController();

        await ipcSendWithId('start-clash');

        // Recharts 渲染会导致 CSS 动画卡顿, 因此要先等待动画完成再触发渲染
        // TODO: 找一个用 canvas 而不是 d3/svg 来渲染的图表库. 待选: chartjs
        await new Promise(resolve => setTimeout(resolve, 200));

        this.props.updateAppConfig(config);
        this.props.serviceReady(controller);

        await this.getCurrentClashConfig();
    }

    componentDidMount(): void {
        window.addEventListener('blur', this.windowBlur);
        window.addEventListener('focus', this.windowFocus);

        this.initialize();
    }

    componentDidUpdate(prevProps: Readonly<AppProps>, prevState: Readonly<AppState>) {
        if (prevProps.location !== this.props.location) {
            this.getCurrentClashConfig();
        }
    }

    componentWillUnmount() {
        window.removeEventListener('blur', this.windowBlur);
        window.removeEventListener('focus', this.windowFocus);
    }

    render() {
        const platformClasses = [
            is.macOS() ? 'is-mac' : 'is-not-mac',
            is.linux() ? 'is-linux' : 'is-not-linux',
            is.windows() ? 'is-windows' : 'is-not-windows',
        ].join(' ');

        const windowClass = this.state.isWindowFocused ? 'window-focus' : 'window-blur';

        const { location } = this.props;

        return (
            <div className={`wrapper ${platformClasses} ${windowClass}`}>
                <Sidebar />

                {this.props.ready ? (
                    <div className="content">
                        {/* Recharts 初次渲染开销很大，所以让它保持在 DOM 中，仅使用 CSS 隐藏 */}
                        <Overview isVisible={location === 'overview'} />

                        {location === 'proxy' && <Proxy />}
                        {location === 'profile' && <Profile />}
                    </div>
                ) : <Loading />}

                <OverlayMessage title="Downloading Maxmind IP Database" isVisible={this.state.downloadingGeoLite}>
                    <Progress ratio={this.state.downloadingProgress} />
                </OverlayMessage>
            </div>
        );
    }
}

const mapStateToProps = (state: GlobalState) => ({
    ready: state.ready,
    location: state.location,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({
    updateConfig(status: ServiceStatus, config: ClashConfig) {
        dispatch({
            type: ActionTypes.UPDATE_CONFIG,
            status, config,
        });
    },
    updateAppConfig(appConfig: Config) {
        dispatch({
            type: ActionTypes.UPDATE_APP_CONFIG,
            appConfig,
        });
    },
    serviceReady(controller: ClashController) {
        dispatch({
            type: ActionTypes.SERVICE_READY,
            controller,
        });
    },
});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(App);
