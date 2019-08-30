import { exec } from 'child_process';
import { ipcMain } from 'electron';
import is from 'electron-is';

async function macGetNetworkServices() {
    const output: string = await new Promise(((resolve) => {
        exec('networksetup -listallnetworkservices', (e, stdout, stderr) => resolve(stdout));
    }));

    return output.split('\n').slice(1).map(srv => srv.trim()).filter(Boolean);
}

async function macSystemProxyStatus() {
    const output: string = await new Promise(((resolve) => {
        exec('networksetup -getwebproxy Wi-Fi', (e, stdout, stderr) => resolve(stdout));
    }));

    return output.split('\n').some(line => line.trim() === 'Enabled: Yes');
}

async function macSystemProxyOn(httpPort: number, socksPort: number) {
    const services = await macGetNetworkServices();

    services.forEach((srv) => {
        if (srv === 'Wi-Fi' || srv.includes('Ethernet') || srv.includes('Airport')) {
            exec([
                `networksetup -setwebproxystate '${srv}' on`,
                `networksetup -setwebproxy '${srv}' 127.0.0.1 ${httpPort}`,
                `networksetup -setsecurewebproxystate '${srv}' on`,
                `networksetup -setsecurewebproxy '${srv}' 127.0.0.1 ${httpPort}`,
                `networksetup -setsocksfirewallproxystate '${srv}' on`,
                `networksetup -setsocksfirewallproxy '${srv}' 127.0.0.1 ${socksPort}`,
            ].join(' && '));
        }
    });
}

async function macSystemProxyOff() {
    const services = await macGetNetworkServices();

    services.forEach((srv) => {
        if (srv === 'Wi-Fi' || srv.includes('Ethernet') || srv.includes('Airport')) {
            exec([
                `networksetup -setwebproxystate '${srv}' off`,
                `networksetup -setsecurewebproxystate '${srv}' off`,
                `networksetup -setsocksfirewallproxystate '${srv}' off`,
            ].join(' && '));
        }
    });
}

export function systemProxyHelper() {
    ipcMain.on('system-proxy-status', (event, id) => {
        const reply = (on: boolean) => event.reply('system-proxy-status-reply-' + id, on);

        if (is.macOS()) {
            macSystemProxyStatus().then(reply);
        }
    });

    ipcMain.on('system-proxy-on', (event, id, httpPort, socksPort) => {
        const reply = () => event.reply('system-proxy-on-reply-' + id);

        if (is.macOS()) {
            macSystemProxyOn(httpPort, socksPort).then(reply);
        }
    });

    ipcMain.on('system-proxy-off', (event, id) => {
        const reply = () => event.reply('system-proxy-off-reply-' + id);

        if (is.macOS()) {
            macSystemProxyOff().then(reply);
        }
    });
}
