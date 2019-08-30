import React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import './Proxy.scss';
import { Config } from '@/types/Config';
import { Group, Proxies, ProxyItem } from '@/types/ClashProxy';
import { ClashConfig } from '@/types/ClashConfig';
import { getConfigFilePath } from '@/util';
import { Action, ActionTypes, GlobalState } from '@/renderer/store';
import { getControllerUrl } from '@/renderer/util';
import { writeAppConfig } from '@/renderer/config';
import PageTitle from '@/renderer/components/PageTitle';

type ProfileProps = {
    config: ClashConfig;
    appConfig: Config;
};

type ProfileState = {
    proxies: Proxies;
};

class Profile extends React.Component<ProfileProps, ProfileState> {
    state = {
        proxies: {},
    };

    async loadProxies() {
        const proxiesUrl = getControllerUrl('/proxies');
        const data = await fetch(proxiesUrl).then(r => r.json());
        this.setState({
            proxies: data.proxies,
        });
    }

    getProxiesShouldDisplay(): [string, Group][] {
        const groupTypes = ['URLTest', 'Fallback', 'LoadBalance', 'Selector'];
        const showGlobal = this.props.config.mode === 'Global';

        const orders = {
            'GLOBAL': 1,
            'Proxy': 2,
        };

        return Object.entries<Group>(this.state.proxies)
            .filter(([name, proxy]) => {
                return name === 'GLOBAL' ? showGlobal : groupTypes.includes(proxy.type);
            })
            .sort((a, b) => {
                const aOrder = orders[a[0]] || 99;
                const bOrder = orders[b[0]] || 99;

                return aOrder - bOrder;
            });
    }

    componentDidMount() {
        this.loadProxies();
    }

    render() {
        const current = this.props.appConfig.configFile;

        return (
            <div className="proxy">
                <PageTitle>Proxy</PageTitle>

                <ul className="clash-proxies">
                    {this.getProxiesShouldDisplay().map(([name, proxy]: [string, Group]) => (
                        <li key={name}>
                            <div>
                                <span className="name">{name}</span>
                                <span className="type">{proxy.type}</span>
                            </div>
                            <div className="description">
                                <i className="ion ion-md-return-right" />
                                {proxy.now}
                            </div>
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

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Profile);
