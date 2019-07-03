import { app, BrowserWindow, ipcMain } from 'electron';
import * as path from 'path';

let mainWindow: Electron.BrowserWindow | null;

function createWindow() {
    if (app.dock) {
        app.dock.show();
    }

    mainWindow = new BrowserWindow({
        frame: false,
        titleBarStyle: 'hiddenInset',
        height: 550,
        width: 900,
        resizable: false,
        maximizable: false,
        vibrancy: 'sidebar',
        backgroundColor: '#00FFFFFF',
        webPreferences: {
            nodeIntegration: true,
        },
    });

    mainWindow.loadFile(path.join(__dirname, '../index.html'));

    mainWindow.on('closed', () => {
        mainWindow = null;
    });
}

function runApp() {
    app.on('ready', createWindow);

    app.on('second-instance', () => {
        if (mainWindow) {
            if (mainWindow.isMinimized()) {
                mainWindow.restore();
            }
            mainWindow.focus();
        } else {
            createWindow();
        }
    });

    app.on('window-all-closed', () => {
        if (app.dock) {
            app.dock.hide();
        }
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
}

if (app.requestSingleInstanceLock()) {
    runApp();
} else {
    app.quit();
}
