import React from 'react';

import './App.scss';
import Sidebar from './Sidebar';
import Content from './Content';

const App = () => (
    <div className="wrapper">
        <Sidebar />
        <Content />
    </div>
);

export default App;
