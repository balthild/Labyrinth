import React from 'react';

import './App.scss';
import Sidebar from './Sidebar';

const App = () => (
    <div className="wrapper">
        <Sidebar />
        <div className="content" />
    </div>
);

export default App;
