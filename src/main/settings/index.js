import { ipcMain } from 'electron';
import _ from 'lodash';
import fs from 'fs-extra';
import { SETTINGS_PATH } from '=/paths';
import SETTINGS_STORE from './store';
import elements from './elements';
import { Save, Load } from './saveLoad';
import setValue from './setValue';


function loadDefaults() {
  return _.mapValues(elements, (value) => {
    const def = value.default;
    return typeof def === 'function' ? def() : def;
  });
}

export async function initialLoad() {
  Object.assign(
    SETTINGS_STORE,
    loadDefaults(),
    await Load(),
  );
  try {
    await fs.stat(SETTINGS_PATH);
  } catch (err) {
    if (err.code === 'ENOENT') {
      Save();
    }
  }
}

/**
 * Hooks to IPC events
 */
export function installIpc() {
  ipcMain.on('settings:load', ({ sender }) => {
    // Send config to renderer
    sender.send('settings:load', SETTINGS_STORE, elements);
  });
  ipcMain.on('settings:set', (event, key, value) => {
    setValue(key, value);
  });
}
