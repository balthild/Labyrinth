import React from 'react';
import is from 'electron-is';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import './Sidebar.scss';
import { Action, GlobalState, ServiceStatus } from '@/renderer/store';
import WindowButtons from './WindowButtons';
import Navigation from './Navigation';

type SidebarProps = {
    clashMode: string;
    status: ServiceStatus;
};

const statusHints = {
    [ServiceStatus.Starting]: ['starting', 'Service Starting'],
    [ServiceStatus.Running]: ['running', 'Service Running'],
    [ServiceStatus.Stopped]: ['stopped', 'Service Stopped'],
};

const Sidebar: React.FC<SidebarProps> = (props) => {
    const [statusClassName, statusText] = statusHints[props.status];

    return (
        <aside className="sidebar">
            {!is.macOS() && <WindowButtons />}

            <div className="service-status">
                <div className={'status-bulb ' + statusClassName} />
                <span className="status-text">{statusText}</span>
            </div>

            <Navigation clashMode={props.clashMode} />
        </aside>
    );
};

const mapStateToProps = (state: GlobalState) => ({
    clashMode: state.config.mode,
    status: state.status,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Sidebar);
