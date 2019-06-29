import store from './store';

export function apiUrl(path: string) {
    const listen = store.getState().apiListen;

    if (!path.startsWith('/')) {
        path = '/' + path;
    }

    return `http://${listen}${path}`;
}
