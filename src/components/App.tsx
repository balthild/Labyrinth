import * as React from 'react';

import './App.scss';
import Sidebar from './Sidebar';

export default class App extends React.Component {
    render() {
        return (
            <div className="wrapper">
                <Sidebar />
                <div className="main" />
            </div>
        );
    }
}
