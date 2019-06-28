import React from 'react';
import os from 'os';

import './Sidebar.scss';
import WindowButtons from './WindowButtons';
import Navigation from './Navigation';

const Sidebar = () => (
    <aside className="sidebar">
        {os.platform() !== 'darwin' && <WindowButtons />}
        <Navigation />
    </aside>
);

export default Sidebar;
