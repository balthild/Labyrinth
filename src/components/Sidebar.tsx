import React from 'react';
import os from 'os';

import './Sidebar.scss';
import WindowButtons from './WindowButtons';
import Navigation from './Navigation';

const platform = os.platform();

const vibrancy = (platform === 'darwin') ? ' vibrancy' : '';

const Sidebar = () => (
    <aside className={'sidebar' + vibrancy}>
        {platform !== 'darwin' && <WindowButtons />}

        <div className="service-status">
            <div className="status-bulb running" />
            <span className="status-text">Service Running</span>
        </div>

        <Navigation />
    </aside>
);

export default Sidebar;
