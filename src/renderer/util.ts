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

export function ipcSendWithId<T>(channel: string, ...args: unknown[]): Promise<T> {
    const id = randomBytes(4).toString('hex');

    return new Promise((resolve) => {
        ipcRenderer.once(`${channel}-reply-${id}`, (event, reply) => resolve(reply));
        ipcRenderer.send(channel, id, ...args);
    });
}

export function capitalizeFirstChar(s: string) {
    return s[0].toUpperCase() + s.slice(1);
}
