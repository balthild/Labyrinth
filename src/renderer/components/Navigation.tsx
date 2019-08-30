import React from 'react';

import './Navigation.scss';
import Item from './NavigationItem';

type NavigationProps = {
    clashMode: string;
};

const Navigation: React.FC<NavigationProps> = (props) => (
    <ul className="app-navigation">
        <Item name="overview" icon="analytics" text="Overview" />
        {props.clashMode !== 'Direct' && <Item name="proxy" icon="navigate" text="Proxy" />}
        <Item name="profile" icon="apps" text="Profile" />
        <Item name="log" icon="time" text="Log" />
        <Item name="settings" icon="hammer" text="Settings" />
        <Item name="help" icon="help-circle" text="Help" />

        <Item name="about" icon="information-circle" text="About" />
    </ul>
);

export default Navigation;
