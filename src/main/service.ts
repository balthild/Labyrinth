import path from 'path';
import { ChildProcess, spawn } from 'child_process';
import { ipcMain } from 'electron';

function getClashExecutablePath() {
    let name = `clash-${process.platform}-${process.arch}`;
    if (process.platform === 'win32') {
        name += '.exe';
    }

    const packed = __filename.split(path.sep).includes('app.asar');
    const resourceDir = packed ? '../..' : '.';

    return path.join(__dirname, resourceDir, 'bin', name);
}

let clashProcess: ChildProcess | null = null;

process.on('exit', () => {
    if (clashProcess) {
        clashProcess.kill();
    }
});

export function serviceReady() {
    ipcMain.on('check-clash-started', (event) => {
        const started = clashProcess && !clashProcess.killed;
        event.reply('check-clash-started-reply', started);
    });

    ipcMain.on('start-clash', (event) => {
        clashProcess = spawn(getClashExecutablePath(), {
            stdio: [null, process.stdout, process.stderr],
        });

        event.reply('start-clash-reply', getClashExecutablePath());
    });

    ipcMain.on('kill-clash', () => {
        if (clashProcess) {
            clashProcess.kill();
            clashProcess = null;
        }
    });
}
