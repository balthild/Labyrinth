import { app, BrowserWindow, ipcMain } from 'electron';
import path from 'path';
import is from 'electron-is';
import { serviceReady } from './service';
import { systemProxyHelper } from '@/main/proxy';

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
        // Remain white background on windows as acrylic material is not available now
        backgroundColor: is.macOS() ? '#00FFFFFF' : '#FFF',
        webPreferences: {
            nodeIntegration: true,
        },
    });

    mainWindow.loadFile(path.join(__dirname, '..', 'index.html'));

    if (is.dev()) {
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
}

if (app.requestSingleInstanceLock()) {
    runApp();
    serviceReady();
    systemProxyHelper();
} else {
    app.quit();
}
