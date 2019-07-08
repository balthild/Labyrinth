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
            <svg viewBox="0 0 10 10">
                <line x1="0.5" y1="0.5" x2="9.5" y2="9.5" />
                <line x1="0.5" y1="9.5" x2="9.5" y2="0.5" />
            </svg>
        </button>

        <button id="window-maximize">
            <svg viewBox="0 0 10 10">
                <line x1="0" y1="0.5" x2="10" y2="0.5" />
                <line x1="0" y1="9.5" x2="10" y2="9.5" />
                <line x1="0.5" y1="0" x2="0.5" y2="10" />
                <line x1="9.5" y1="0" x2="9.5" y2="10" />
            </svg>
        </button>

        <button id="window-minimize" onClick={minimizeWindow}>
            <svg viewBox="0 0 10 10">
                <line x1="0" y1="5" x2="10" y2="5" />
            </svg>
        </button>

    </div>
);

export default WindowButtons;
