import React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import './Proxy.scss';
import { Group, Proxies, Proxy, ProxyItem, Selector } from '@/types/ClashProxy';
import { ClashConfig } from '@/types/ClashConfig';
import { Action, GlobalState } from '@/renderer/store';
import { getControllerUrl } from '@/renderer/util';
import PageTitle from '@/renderer/components/PageTitle';
import Detail from '@/renderer/components/Detail';

type ProfileProps = {
    config: ClashConfig;
};

type ProfileState = {
    proxies: Proxies;
    showDetail: boolean;
    detailName: string;
};

class Profile extends React.Component<ProfileProps, ProfileState> {
    state = {
        proxies: {} as Proxies,
        showDetail: false,
        detailName: '',
    };

    showDetail = async (e: React.MouseEvent<HTMLLIElement>) => {
        const name = e.currentTarget.dataset.name!!;

        await this.loadProxies();

        this.setState({
            showDetail: true,
            detailName: name,
        });
    };

    hideDetail = async (e: React.MouseEvent) => {
        await this.loadProxies();

        this.setState({
            showDetail: false,
        });
    };

    setSelectorProxy = async (e: React.MouseEvent<HTMLElement>) => {
        const { selector, proxy } = e.currentTarget.dataset;

        const proxiesUrl = getControllerUrl(`/proxies/${selector}`);
        await fetch(proxiesUrl, {
            method: 'PUT',
            body: JSON.stringify({
                name: proxy,
            }),
        });

        await this.loadProxies();
    };

    async loadProxies() {
        const proxiesUrl = getControllerUrl('/proxies');
        const data = await fetch(proxiesUrl).then(r => r.json());
        this.setState({
            proxies: data.proxies,
        });
    }

    getProxiesShouldDisplay() {
        const showGlobal = this.props.config.mode === 'Global';
        const orders = {
            'GLOBAL': 1,
            'Proxy': 2,
        };

        const groupEntries = Object.entries<ProxyItem>(this.state.proxies)
            .filter(([name, proxy]) => {
                return name === 'GLOBAL' ? showGlobal : groupTypes.includes(proxy.type);
            })
            .sort((a, b) => {
                const aOrder = orders[a[0]] || 99;
                const bOrder = orders[b[0]] || 99;

                return aOrder - bOrder;
            });

        return groupEntries as [string, Group][];
    }

    componentDidMount() {
        this.loadProxies();
    }

    renderGroupDetail(groupName: string, group?: Group) {
        if (!group) {
            return null;
        }

        const { proxies } = this.state;

        const isSelector = group.type === 'Selector';

        let groupProxies = group.all;
        if (groupName === 'GLOBAL') {
            const typeOrders = {
                'Global': 1,
                'Direct': 2,
                'Reject': 3,
                'URLTest': 10,
                'Fallback': 10,
                'LoadBalance': 10,
                'Selector': 10,
            };

            groupProxies = group.all.sort((a, b) => {
                const aType = proxies[a].type;
                const bType = proxies[b].type;

                const aOrder = typeOrders[aType] || 99;
                const bOrder = typeOrders[bType] || 99;

                return aOrder - bOrder;
            });
        }

        return (
            <ul className="clash-proxies">
                {groupProxies.map((proxyName) => {
                    const proxy = proxies[proxyName];
                    const isGroup = groupTypes.includes(proxy.type);

                    const selectorSwitcherProps = isSelector ? {
                        'data-selector': groupName,
                        'data-proxy': proxyName,
                        onClick: this.setSelectorProxy,
                    } : {};

                    return (
                        <li key={proxyName} className={proxyName === group.now ? 'current' : ''}>
                            <div>
                                <span className="name">{proxyName}</span>
                                <span className="type">{proxies[proxyName].type}</span>
                            </div>
                            <div className="description">
                                {isGroup ? (
                                    <>
                                        <i className="ion ion-md-return-right" />
                                        {(proxy as Group).now}
                                    </>
                                ) : (proxy as Proxy).history.slice(0, 3).map((h) => (
                                    <span key={h.time} className="delay">{h.delay}ms</span>
                                ))}
                            </div>

                            {isSelector && (
                                <div className="active-sign" {...selectorSwitcherProps}>
                                    <i className={`ion ${activeSignIconClass(proxyName, group.now)}`} />
                                </div>
                            )}
                        </li>
                    );
                })}
            </ul>
        );
    }

    render() {
        const { showDetail, detailName, proxies } = this.state;

        const detailItem = proxies[detailName] as Group;

        return (
            <>
                <div className="proxy content-inner">
                    <PageTitle>Proxy</PageTitle>

                    <ul className="clash-proxies">
                        {this.getProxiesShouldDisplay().map(([name, proxy]) => (
                            <li key={name} data-name={name} onClick={this.showDetail}>
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

                <Detail isOpened={showDetail} title={detailName} onCloseButtonClick={this.hideDetail}>
                    {this.renderGroupDetail(detailName, detailItem)}
                </Detail>
            </>
        );
    }
}

const groupTypes = ['URLTest', 'Fallback', 'LoadBalance', 'Selector'];

const activeSignIconClass = (config: string, currentConfig: string) =>
    config === currentConfig ?
        'ion-ios-checkmark-circle' :
        'ion-ios-checkmark-circle-outline';

const mapStateToProps = (state: GlobalState) => ({
    config: state.config,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Profile);
