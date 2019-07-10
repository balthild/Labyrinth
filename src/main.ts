import { app, BrowserWindow, ipcMain } from 'electron';
import * as path from 'path';
import { serviceReady } from '@/service';

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

    if (process.env.NODE_ENV === 'development') {
        mainWindow.webContents.openDevTools({ mode: 'detach' });
    }

    mainWindow.on('closed', () => {
        mainWindow = null;
    });
}

function presentWindow() {
    if (mainWindow) {
        if (mainWindow.isMinimized()) {
            mainWindow.restore();
        }
        mainWindow.focus();
    } else {
        createWindow();
    }
}

function runApp() {
    app.on('ready', createWindow);
    app.on('second-instance', presentWindow);
    app.on('activate', presentWindow);

    app.on('window-all-closed', () => {
        if (app.dock) {
            app.dock.hide();
        }
    });

    ipcMain.on('window-close', () => {
        if (mainWindow !== null) {
            mainWindow.close();
        }
    });

    ipcMain.on('window-minimize', () => {
        if (mainWindow !== null) {
            mainWindow.minimize();
        }
    });

    serviceReady();
}

if (app.requestSingleInstanceLock()) {
    runApp();
} else {
    app.quit();
}
