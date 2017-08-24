import { ipcMain } from 'electron';
import _ from 'lodash';
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
  _.each(SETTINGS_STORE, (value, key) => {
    if (!elements[key]) {
      delete SETTINGS_STORE[key];
    }
  });
  // Save config, so we can add default values for new unmodified properties
  Save();
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
