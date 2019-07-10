import React from 'react';
import { ipcRenderer } from 'electron';

import './WindowButtons.scss';

function closeWindow() {
    ipcRenderer.send('window-close');
}

function minimizeWindow() {
    ipcRenderer.send('window-minimize');
}

const WindowButtons = () => (
    <div className="window-buttons">
        <button id="window-close" onClick={closeWindow}>
            <svg viewBox="0 0 10 10">
                <line x1="0.5" y1="0.5" x2="9.5" y2="9.5" />
                <line x1="0.5" y1="9.5" x2="9.5" y2="0.5" />
            </svg>
        </button>

        <button id="window-maximize" disabled>
            <svg viewBox="0 0 10 10">
                <rect width={9} height={9} x={0.5} y={0.5} fill="none" />
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
