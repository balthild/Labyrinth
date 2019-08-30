import { exec } from 'child_process';
import { ipcMain } from 'electron';
import is from 'electron-is';

async function macGetNetworkServices() {
    const output: string = await new Promise(((resolve) => {
        exec('networksetup -listallnetworkservices', (e, stdout, stderr) => resolve(stdout));
    }));

    return output.split('\n').slice(1).filter(srv => srv.trim());
}

async function macSystemProxyStatus() {
    const output: string = await new Promise(((resolve) => {
        exec('networksetup -getwebproxy Wi-Fi', (e, stdout, stderr) => resolve(stdout));
    }));

    const statusText = output.split('\n')[0].trim();

    return statusText === 'Enabled: Yes';
}

async function macSystemProxyOn(httpPort: number, socksPort: number) {
    const services = await macGetNetworkServices();

    services.forEach((srv) => {
        if (srv === 'Wi-Fi' || srv.includes('Ethernet') || srv.includes('Airport')) {
            exec(`networksetup -setwebproxystate '${srv}' on`);
            exec(`networksetup -setwebproxy '${srv}' 127.0.0.1 ${httpPort}`);
            exec(`networksetup -setsecurewebproxystate '${srv}' on`);
            exec(`networksetup -setsecurewebproxy '${srv}' 127.0.0.1 ${httpPort}`);
            exec(`networksetup -setsocksfirewallproxystate '${srv}' on`);
            exec(`networksetup -setsocksfirewallproxy '${srv}' 127.0.0.1 ${socksPort}`);
        }
    });
}

async function macSystemProxyOff() {
    const services = await macGetNetworkServices();

    services.forEach((srv) => {
        if (srv === 'Wi-Fi' || srv.includes('Ethernet') || srv.includes('Airport')) {
            exec(`networksetup -setwebproxystate '${srv}' off`);
            exec(`networksetup -setsecurewebproxystate '${srv}' off`);
            exec(`networksetup -setsocksfirewallproxystate '${srv}' off`);
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
