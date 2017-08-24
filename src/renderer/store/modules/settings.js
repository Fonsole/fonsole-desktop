/**
 * @file Functions to work with stored fonsole configuration.
 */

// @ifdef ELECTRON
import { ipcRenderer } from 'electron';
// @endif

const mutations = {
  /**
   * Recursively updates all keys of current settings.
   *
   * @param {any} state Vuex state
   * @param {Object} settings Settings object
   */
  setConfig(state, settings) {
    // Assign all keys of new settings to old, so we can trigger reactivity
    Object.assign(state, settings);
    // Delete keys that are present in old settings, but not present in new settings.
    Object.keys(state)
      .filter(key => settings[key] == null)
      .forEach((key) => {
        delete state[key];
      });
  },

  /**
   * Set setting key in store to specific value.
   *
   * @param {any} state Vuex state
   * @param {string} payload.key Setting key
   * @param {any} payload.value New setting value
   */
  setValue(state, { key, value }) {
    state[key] = value;
  },
};

const actions = {
  /**
   * Load saved settings from file system.
   *
   * @param {Vuex.Store} store Vuex store
   */
  init({ commit }) {
    // @ifdef ELECTRON
    ipcRenderer.on('settings:load', (event, settings) => {
      commit('setConfig', settings);
    });
    ipcRenderer.send('settings:load');
    // @endif
    // @ifdef WEB
    commit('setConfig', JSON.parse(localStorage.settings || '{}'));
    // @endif
  },

  /**
   * Set setting key to specific value.
   * Also updates saved settings in file system.
   * Works either in browser and electron.
   *
   * @param {Vuex.Store} store Vuex store
   * @param {string} payload.key Setting key
   * @param {any} payload.value New setting value
   */
  setValue({ state, commit }, { key, value }) {
    // Set new value in local settings
    commit('setValue', {
      key,
      value,
    });
    // @ifdef ELECTRON
    // Update value in file system
    ipcRenderer.send('settings:set', key, value);
    // @endif
    // @ifdef WEB
    // Update locally stored settings
    localStorage.settings = state;
    // @endif
  },
};

export default {
  state: {},
  actions,
  mutations,
};
