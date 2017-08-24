import { app, BrowserWindow } from 'electron';
import ipcModules from './ipcModules';
import { installIpc, initialLoad } from './settings';
import SETTINGS_STORE from './settings/store';
import setValue from './settings/setValue';
import bounds from './settings/elements/bounds';

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
  const defaultBounds = bounds.default();
  // Initial window options
  mainWindow = new BrowserWindow({
    minWidth: 400,
    minHeight: 300,
    width: defaultBounds.width,
    height: defaultBounds.height,
  });
  mainWindow.setFullScreen(SETTINGS_STORE.fullscreen);
  if (SETTINGS_STORE.maximized) mainWindow.maximize();
  if (SETTINGS_STORE.bounds) {
    mainWindow.maximize();
    mainWindow.setBounds(SETTINGS_STORE.bounds);
  }
  mainWindow.setMenu(null);
  mainWindow.loadURL(winURL);

  ipcModules();
  installIpc();

  mainWindow.on('close', () => {
    setValue('bounds', mainWindow.getBounds());
    setValue('maximized', mainWindow.isMaximized());
  });

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
