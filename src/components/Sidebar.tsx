import * as React from 'react';

import './Sidebar.scss';
import WindowButtons from './WindowButtons';

export default class Sidebar extends React.Component {
    render() {
        return (
            <aside className="sidebar">
                <WindowButtons />
            </aside>
        );
    }
}
