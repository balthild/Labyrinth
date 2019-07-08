import React from 'react';
import is from 'electron-is';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import './App.scss';
import { Config } from '@/types/Config';
import { Action, ActionTypes, GlobalState, ServiceStatus } from '../store';
import Sidebar from './Sidebar';
import Loading from './Loading';
import Content from './Content';

type AppProps = {
    ready: boolean;
    initialize(status: ServiceStatus, config: Config): void;
};

class App extends React.Component<AppProps> {
    async initialize() {
        // TODO
        this.props.initialize(ServiceStatus.Running, {
            'port': 0,
            'socket-port': 0,
            'redir-port': 0,
            'allow-lan': false,
            'mode': 'Rule',
            'log-level': 'info',
            'external-controller': 'localhost:9090',
        });
    }

    componentDidMount(): void {
        this.initialize();
    }

    render() {
        const platformClasses = [
            is.macOS() ? 'is-mac' : 'is-not-mac',
            is.linux() ? 'is-linux' : 'is-not-linux',
            is.windows() ? 'is-windows' : 'is-not-windows',
        ].join(' ');

        return (
            <div className={'wrapper ' + platformClasses}>
                <Sidebar />
                {this.props.ready ? <Content /> : <Loading />}
            </div>
        );
    }
}

const mapStateToProps = (state: GlobalState) => ({
    ready: state.ready,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({
    initialize(status: ServiceStatus, config: Config) {
        dispatch({
            type: ActionTypes.UPDATE_SERVICE,
            status, config,
        });

        dispatch({
            type: ActionTypes.APP_READY,
        });
    },
});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(App);
