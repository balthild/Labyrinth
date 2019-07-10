import React from 'react';
import http from 'http';
import readline from 'readline';
import { Area, AreaChart, CartesianGrid, Legend, ResponsiveContainer, YAxis } from 'recharts';
import fileSize from 'filesize';
import isPropValid from '@emotion/is-prop-valid';

import './Overview.scss';
import { getControllerUrl } from '@/renderer/util';

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
};

type OverviewState = {
    traffics: TrafficEntry[];
    currentTraffic: number;
};

class Overview extends React.Component<OverviewProps, OverviewState> {
    state = {
        traffics: new Array(120).fill({ up: 0, down: 0 }),
        currentTraffic: 0,
    };

    dead = false;

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

        return (
            <div className="overview" style={this.props.isVisible ? undefined : { display: 'none' }}>
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
            </div>
        );
    }
}

export default Overview;
