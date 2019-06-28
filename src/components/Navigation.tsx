import React from 'react';

import './Navigation.scss';
import Item from './NavigationItem';

const Navigation = () => (
    <ul className="app-navigation">
        <Item name="overview" icon="speedometer" text="Overview" />
        <Item name="proxy" icon="navigate" text="Proxy" />
        <Item name="profile" icon="apps" text="Profile" />
    </ul>
);

export default Navigation;
