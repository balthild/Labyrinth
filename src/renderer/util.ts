import { ipcRenderer } from 'electron';

import store, { GlobalState } from './store';

export function getControllerUrl(path: string): string {
    const state: GlobalState = store.getState();
    const listen = state.controller['external-controller'];

    if (!path.startsWith('/')) {
        path = '/' + path;
    }

    return `http://${listen}${path}`;
}

export async function startClash() {
    const started = await new Promise((resolve) => {
        ipcRenderer.once('check-clash-started-reply', (event, reply: boolean) => {
            resolve(reply);
        });
        ipcRenderer.send('check-clash-started');
    });

    if (started) {
        return;
    }

    await new Promise((resolve) => {
        ipcRenderer.once('start-clash-reply', (event, reply: string) => {
            resolve(reply);
        });
        ipcRenderer.send('start-clash');
    });
}
