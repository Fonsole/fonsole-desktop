import elements from './elements';
import { Save } from './saveLoad';
import SETTINGS_STORE from './store';

/**
 * Set configuration key to specific value.
 * Also updates saved config in file system.
 *
 * @param {string} key Configuration key
 * @param {any} value New configuration value
 */
export default function setValue(key, value, shouldSave) {
  // Make sure that setting exists
  const setting = elements[key];
  const oldValue = SETTINGS_STORE[key];
  if (setting) {
    // Make sure that new value either fulfills validator or this setting hasn't it
    if (setting.validator == null || setting.validator(value, oldValue)) {
      // If setting has a list of allowed variants => check that value is valid
      if (!Array.isArray(setting.variants) || setting.variants.includes(value)) {
        // If we handle change in setting => call handler
        if (setting.changed) setting.changed(value, oldValue);
        // Finally update singleton key
        SETTINGS_STORE[key] = value;
        if (shouldSave !== false) {
          // Save it to disk
          Save();
        }
      }
    }
  }
}
