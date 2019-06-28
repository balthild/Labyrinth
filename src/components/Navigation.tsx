import React from 'react';

import './Navigation.scss';
import Item from './NavigationItem';

const Navigation = () => (
    <ul className="app-navigation">
        <Item name="overview" icon="analytics" text="Overview" />
        <Item name="proxy" icon="navigate" text="Proxy" />
        <Item name="profile" icon="apps" text="Profile" />
        <Item name="log" icon="time" text="Log" />
        <Item name="settings" icon="hammer" text="Settings" />
        <Item name="help" icon="help-circle" text="Help" />

        <Item name="about" icon="information-circle" text="About" />
    </ul>
);

export default Navigation;
