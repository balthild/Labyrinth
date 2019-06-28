import React from 'react';
import { ipcRenderer } from 'electron';

import './WindowButtons.scss';

function closeWindow() {
    ipcRenderer.send('close');
}

function minimizeWindow() {
    ipcRenderer.send('minimize');
}

const WindowButtons = () => (
    <div className="window-buttons">
        <button id="window-close" onClick={closeWindow}>
            <i className="sign ion ion-md-close" />
        </button>
        <button id="window-minimize">
            <i className="sign ion ion-md-remove" onClick={minimizeWindow} />
        </button>
        <button />
    </div>
);

export default WindowButtons;
