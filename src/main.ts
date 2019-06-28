import { app, BrowserWindow, ipcMain } from 'electron';
import * as path from 'path';

let mainWindow: Electron.BrowserWindow | null;

function createWindow() {
    app.dock.show();

    mainWindow = new BrowserWindow({
        frame: false,
        titleBarStyle: 'hiddenInset',
        height: 625,
        width: 1000,
        resizable: false,
        maximizable: false,
        webPreferences: {
            nodeIntegration: true,
        },
    });

    mainWindow.loadFile(path.join(__dirname, '../index.html'));

    mainWindow.webContents.openDevTools({ mode: 'detach' });

    mainWindow.on('closed', () => {
        mainWindow = null;
    });
}

app.on('ready', createWindow);

app.on('window-all-closed', () => {
    app.dock.hide();
});

app.on('activate', () => {
    if (mainWindow === null) {
        createWindow();
    }
});

ipcMain.on('close', () => {
    if (mainWindow !== null) {
        mainWindow.close();
    }
});

ipcMain.on('minimize', () => {
    if (mainWindow !== null) {
        mainWindow.minimize();
    }
});
