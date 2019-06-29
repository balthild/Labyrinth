import React from 'react';
import http from 'http';
import readline from 'readline';
import { Area, AreaChart, CartesianGrid, Legend, Line, LineChart, YAxis } from 'recharts';
import fileSize from 'filesize';

import './Overview.scss';
import { apiUrl } from '@/util';

type TrafficEntry = {
    up: number;
    down: number;
};

type OverviewProps = {
};

type OverviewState = {
    traffics: TrafficEntry[];
};

class Overview extends React.Component<OverviewProps, OverviewState> {
    state = {
        traffics: new Array(60).fill({ up: 0, down: 0 }),
    };

    dead = false;

    async updateTrafficCharts() {
        await new Promise(resolve => {
            const client = http.get(apiUrl('/traffic'), (response) => {
                response.on('end', resolve);

                const reader = readline.createInterface({
                    input: response,
                });

                reader.on('line', (chunk: Buffer) => {
                    if (this.dead) {
                        response.destroy();
                        return;
                    }

                    const entry = JSON.parse(chunk.toString());

                    this.setState((prevState) => {
                        const traffics = prevState.traffics.slice(1);
                        traffics.push(entry);

                        return { traffics };
                    });
                });
            });

            client.on('error', resolve);
        });

        if (this.dead) {
            return;
        }

        await new Promise(resolve => setTimeout(resolve, 1000));

        this.updateTrafficCharts();
    }

    componentDidMount() {
        this.dead = false;
        this.updateTrafficCharts();
    }

    componentWillUnmount() {
        this.dead = true;
    }

    render() {
        return (
            <div className="overview">
                <div className="banner">
                    <div className="info">
                        <p>asdasd</p>
                    </div>

                    <AreaChart width={450} height={240} data={this.state.traffics}>
                        <CartesianGrid stroke="#eee" strokeDasharray="5 5" vertical={false} />
                        <Legend layout="vertical" width={1} align="right" verticalAlign="top" />

                        <Area
                            type="monotoneX"
                            dataKey="up"
                            stackId={1}
                            fill="#8884d8"
                            stroke="#8884d8"
                            strokeWidth={0}
                            dot={false}
                            isAnimationActive={false}
                        />
                        <Area
                            type="monotoneX"
                            dataKey="down"
                            stackId={1}
                            fill="#82ca9d"
                            stroke="#82ca9d"
                            strokeWidth={0}
                            dot={false}
                            isAnimationActive={false}
                        />

                        <YAxis
                            width={1}
                            axisLine={false}
                            tickLine={false}
                            tick={{ dx: 8, dy: 8, fill: 'rgba(0, 0, 0, 0.4)', fontSize: 10, textAnchor: 'start' }}
                            tickFormatter={formatTraffic}
                        />
                    </AreaChart>
                </div>
            </div>
        );
    }
}

function formatTraffic(byte: number) {
    const size = fileSize(byte, { spacer: '', standard: 'iec', round: 0 });
    return size + '/s';
}

export default Overview;
