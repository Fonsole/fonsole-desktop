import { app, BrowserWindow } from 'electron';
import { stringToResolution } from '=/util';
import ipcModules from './ipcModules';
import { installIpc, initialLoad } from './settings';
import SETTINGS_STORE from './settings/store';

/**
 * Set `__static` path to static files in production
 * https://simulatedgreg.gitbooks.io/electron-vue/content/en/using-static-assets.html
 */
if (process.env.NODE_ENV !== 'development') {
  // eslint-disable-next-line no-underscore-dangle
  global.__static = require('path').join(__dirname, '/static').replace(/\\/g, '\\\\');
}

let mainWindow;
const winURL = process.env.NODE_ENV === 'development'
  ? 'http://localhost:8080'
  : `file://${__dirname}/index.html`;

async function createWindow() {
  await initialLoad();
  const resolution = stringToResolution(SETTINGS_STORE.resolution);
  // Initial window options
  mainWindow = new BrowserWindow({
    width: resolution[0],
    height: resolution[1],
    resizable: false,
  });
  mainWindow.setMenu(null);
  mainWindow.loadURL(winURL);

  ipcModules();
  installIpc();

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

app.on('ready', createWindow);

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', () => {
  if (mainWindow === null) {
    createWindow();
  }
});
