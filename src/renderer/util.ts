import { ipcRenderer } from 'electron';
import { randomBytes } from 'crypto';

import store, { GlobalState } from './store';

export function getControllerUrl(path: string): string {
    const state: GlobalState = store.getState();
    const listen = state.controller['external-controller'];

    if (!path.startsWith('/')) {
        path = '/' + path;
    }

    return `http://${listen}${path}`;
}

export function startClash() {
    const id = randomBytes(4).toString('hex');

    return new Promise((resolve) => {
        ipcRenderer.once('start-clash-reply-' + id, resolve);
        ipcRenderer.send('start-clash', id);
    });
}

export function capitalizeFirstChar(s: string) {
    return s[0].toUpperCase() + s.slice(1);
}
