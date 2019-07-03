import React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import os from 'os';

import './Sidebar.scss';
import { Action, GlobalState, ServiceStatus } from '../store';
import WindowButtons from './WindowButtons';
import Navigation from './Navigation';

const platform = os.platform();

const vibrancy = (platform === 'darwin') ? ' vibrancy' : '';

type SidebarProps = {
    status: ServiceStatus,
};

const Sidebar: React.FunctionComponent<SidebarProps> = (props) => {
    let statusClassName, statusText;
    switch (props.status) {
        case ServiceStatus.Starting:
            statusClassName = 'starting';
            statusText = 'Service Starting';
            break;

        case ServiceStatus.Running:
            statusClassName = 'running';
            statusText = 'Service Running';
            break;

        case ServiceStatus.Stopped:
            statusClassName = 'stopped';
            statusText = 'Service Stopped';
            break;

        default: // Unreachable
    }

    return (
        <aside className={'sidebar' + vibrancy}>
            {platform !== 'darwin' && <WindowButtons />}

            <div className="service-status">
                <div className={'status-bulb ' + statusClassName} />
                <span className="status-text">{statusText}</span>
            </div>

            <Navigation />
        </aside>
    );
};

const mapStateToProps = (state: GlobalState) => ({
    status: state.status,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Sidebar);
