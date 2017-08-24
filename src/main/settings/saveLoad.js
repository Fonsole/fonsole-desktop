import fs from 'fs-extra';
import yaml from 'js-yaml';
import { SETTINGS_PATH } from '=/paths';
import SETTINGS_STORE from './store';

// Set this to true while writing config file, so we can don't trigger watcher for our events.
let WRITING = false;

/**
 * Load current config from file system.
 *
 * @async
 * @returns Promise
 */
export async function Load() {
  let file;
  try {
    await fs.stat(SETTINGS_PATH);
    file = await fs.readFile(SETTINGS_PATH, 'utf8');
  } catch (err) {
    if (err.code === 'ENOENT') {
      // Config file not exists
      return {};
    }
    // TODO Handle other errors
  }
  try {
    return yaml.safeLoad(file);
  } catch (err) {
    if (err instanceof yaml.YAMLException) {
      // TODO Broken config
    }
    return {};
  }
}

/**
 * Save current config state to file system.
 */
export async function Save() {
  // Stringify our settings object to yaml
  const data = yaml.safeDump(SETTINGS_STORE);
  try {
    // Set writing state, so we can don't trigger fs watcher.
    WRITING = true;
    // Save configuration to config file
    await fs.writeFile(SETTINGS_PATH, data);
    //
    WRITING = false;
  } catch (err) {
    // TODO Handle errors
  }
}

// Set up a watcher, so we can update config if user changes it while fonsole is running
fs.watchFile(SETTINGS_PATH, () => {
  if (!WRITING) Load();
});
