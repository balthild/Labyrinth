import React from 'react';
import http from 'http';
import readline from 'readline';
import { Area, AreaChart, CartesianGrid, Legend, ResponsiveContainer, YAxis } from 'recharts';
import fileSize from 'filesize';
import isPropValid from '@emotion/is-prop-valid';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import './Overview.scss';
import { capitalizeFirstChar, getControllerUrl, ipcSendWithId } from '@/renderer/util';
import { Action, ActionTypes, GlobalState, ServiceStatus } from '@/renderer/store';
import { ClashConfig } from '@/types/ClashConfig';
import Switch from '@/renderer/components/Switch';

type AxisTickTextProps = {
    payload: {
        value: number;
    };
    y: number;
};

function AxisTickText(props: AxisTickTextProps) {
    const passingValidProps = {};
    Object.keys(props)
        .filter(isPropValid)
        .forEach(key => passingValidProps[key] = props[key]);

    const textProps = {
        ...passingValidProps,
        dx: -16,
        // Magic numbers, don't touch
        dy: props.y < 12 ? 7 : 12,
        fill: 'rgba(255, 255, 255, 0.3)',
        fontSize: 10,
        textAnchor: 'end',
    };

    const size = fileSize(props.payload.value, {
        standard: 'iec',
        round: 1,
    });

    return <text {...textProps}>{size}/s</text>;
}

type TrafficEntry = {
    up: number;
    down: number;
};

type OverviewProps = {
    isVisible: boolean;
    config: ClashConfig;
    updateConfig(status: ServiceStatus, config: ClashConfig): void;
};

type OverviewState = {
    traffics: TrafficEntry[];
    currentTraffic: number;
    isSystemProxySet: boolean;
};

class Overview extends React.Component<OverviewProps, OverviewState> {
    state = {
        traffics: new Array(120).fill({ up: 0, down: 0 }),
        currentTraffic: 0,
        isSystemProxySet: false,
    };

    dead = false;

    setMode = async (e: React.MouseEvent<HTMLDivElement>) => {
        const mode = e.currentTarget.textContent!!;

        if (mode === this.props.config.mode) {
            return;
        }

        const configUrl = getControllerUrl('/configs');
        await fetch(configUrl, {
            method: 'PATCH',
            body: JSON.stringify({ mode }),
        });

        this.props.updateConfig(ServiceStatus.Running, {
            ...this.props.config,
            mode,
        });
    };

    toggleAllowLan = async () => {
        const allow = !this.props.config['allow-lan'];

        const configUrl = getControllerUrl('/configs');
        await fetch(configUrl, {
            method: 'PATCH',
            body: JSON.stringify({
                'allow-lan': allow,
            }),
        });

        this.props.updateConfig(ServiceStatus.Running, {
            ...this.props.config,
            'allow-lan': allow,
        });
    };

    toggleSystemProxy = async () => {
        const currentOn = this.state.isSystemProxySet;

        if (currentOn) {
            await ipcSendWithId('system-proxy-off');
        } else {
            const { 'port': httpPort, 'socks-port': socksPort } = this.props.config;
            await ipcSendWithId('system-proxy-on', httpPort, socksPort);
        }

        this.setState({
            isSystemProxySet: !currentOn,
        });
    };

    async getSystemProxyStatus() {
        const on = await ipcSendWithId<boolean>('system-proxy-status');

        this.setState({
            isSystemProxySet: on,
        });
    }

    async updateTrafficCharts() {
        await new Promise((resolve) => {
            const client = http.get(getControllerUrl('/traffic'), (response) => {
                response.on('end', resolve);

                const reader = readline.createInterface({
                    input: response,
                });

                reader.on('line', (chunk: Buffer) => {
                    if (this.dead) {
                        response.destroy();
                        return;
                    }

                    const entry: TrafficEntry = JSON.parse(chunk.toString());

                    this.setState((prevState) => {
                        const traffics = prevState.traffics.slice(1);
                        traffics.push(entry);

                        return {
                            traffics,
                            currentTraffic: entry.up + entry.down,
                        };
                    });
                });
            });

            client.on('error', resolve);
        });

        if (this.dead) {
            return;
        }

        await new Promise(resolve => setTimeout(resolve, 1000));

        this.setState({
            currentTraffic: 0,
        });

        this.updateTrafficCharts();
    }

    componentDidMount() {
        this.dead = false;
        this.getSystemProxyStatus();
        this.updateTrafficCharts();
    }

    shouldComponentUpdate(nextProps: Readonly<OverviewProps>, nextState: Readonly<OverviewState>) {
        return !this.dead && this.props.isVisible || nextProps.isVisible;
    }

    componentWillUnmount() {
        this.dead = true;
    }

    render() {
        const traffic = fileSize(this.state.currentTraffic, {
            standard: 'iec',
            round: 1,
        });

        const currentMode = capitalizeFirstChar(this.props.config.mode);
        const modeClass = (mode: string) => currentMode === mode ? 'current' : '';

        return (
            <div className="overview content-inner" style={this.props.isVisible ? undefined : { display: 'none' }}>
                <div className="banner">
                    <div className="info">
                        <div className="label">TRAFFIC</div>
                        <div className="traffic">{traffic}/s</div>
                    </div>

                    <ResponsiveContainer>
                        <AreaChart height={240} margin={{ bottom: 0 }} data={this.state.traffics}>
                            <CartesianGrid stroke="rgba(255, 255, 255, 0.08)" strokeDasharray="5 5" vertical={false} />
                            <Legend layout="horizontal" align="center" verticalAlign="top" />

                            <Area
                                type="monotone"
                                dataKey="up"
                                stackId={1}
                                fill="#80d134"
                                stroke="#80d134"
                                strokeWidth={0}
                                dot={false}
                                isAnimationActive={false}
                            />
                            <Area
                                type="monotone"
                                dataKey="down"
                                stackId={1}
                                fill="#1da2d8"
                                stroke="#1da2d8"
                                strokeWidth={0}
                                dot={false}
                                isAnimationActive={false}
                            />

                            <YAxis
                                width={0.0001}
                                orientation="right"
                                axisLine={false}
                                tickLine={false}
                                tick={AxisTickText}
                            />
                        </AreaChart>
                    </ResponsiveContainer>
                </div>

                <div className="settings-row">
                    <div className="settings-panel">
                        <div className="label">MODE</div>

                        <div className="current-mode">{currentMode}</div>

                        <div className="mode-switcher">
                            <div className={`mode ${modeClass('Rule')}`} onClick={this.setMode}>Rule</div>
                            <div className={`mode ${modeClass('Global')}`} onClick={this.setMode}>Global</div>
                            <div className={`mode ${modeClass('Direct')}`} onClick={this.setMode}>Direct</div>
                        </div>
                    </div>

                    <div className="settings-panel">
                        <div className="label">PROXY</div>

                        <div className="toggle-setting">
                            <span>Allow LAN</span>
                            <Switch on={this.props.config['allow-lan']} onClick={this.toggleAllowLan} />
                        </div>

                        <div className="toggle-setting">
                            <span>System Proxy</span>
                            <Switch on={this.state.isSystemProxySet} onClick={this.toggleSystemProxy} />
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

const mapStateToProps = (state: GlobalState) => ({
    config: state.config,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({
    updateConfig(status: ServiceStatus, config: ClashConfig) {
        dispatch({
            type: ActionTypes.UPDATE_CONFIG,
            status, config,
        });
    },
});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Overview);
