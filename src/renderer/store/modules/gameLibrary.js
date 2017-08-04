/**
 * @file This file implements everything that is used to communicate with local game library.
 */

import _ from 'lodash';
import { GAME_STATUS } from '=/enums';

const { ipcRenderer } = process.env.IS_WEB ? {} : require('electron');

const initialState = {
  gameStatuses: {},
  gameDownloadProgresses: {},
};

const getters = {
  getGameStatus: state => id => state.gameStatuses[id] || GAME_STATUS.UNINSTALLED,
  getGameDownloadProgress: state => id => state.gameDownloadProgresses[id],
  getInstalledGameIds: state => Object.keys(_.pickBy(state.gameStatuses,
    (id, status) => status === GAME_STATUS.INSTALLED || GAME_STATUS.UPDATABLE)),
};

const mutations = {
  /**
   * Update game status
   *
   * @param {any} state Vuex state
   * @param {number} payload.id Game ID
   * @param {GAME_STATUS} payload.status Desired status
   */
  setGameStatus(state, payload) {
    state.gameStatuses[payload.id] = payload.status;
  },

  /**
   * Update game download progress
   *
   * @param {any} state Vuex state
   * @param {number} payload.id Game ID
   * @param {object} payload.progress New download progress
   */
  setGameDownloadProgress(state, payload) {
    state.gameDownloadProgresses[payload.id] = payload.progress;
  },
};

const actions = {
  /**
   * Initially load all games and subscribe to internal IPC events
   *
   * @todo Implement error handling for 'gameLibrary:game:status' event
   * @param {Vuex.Store} store Vuex store
   */
  init(store) {
    if (!ipcRenderer) {
      // TODO Maybe give some predefined list?
      return;
    }
    // Synchronously get all downloaded games.
    // Everything else will be handled asynchronously.
    const games = ipcRenderer.sendSync('gameLibrary:library:load');
    games.forEach((id) => {
      store.commit('setGameStatus', {
        id,
        status: GAME_STATUS.INSTALLED,
      });
    });

    // Initialized, subscribe to events
    // Main process wants to update game status
    ipcRenderer.on('gameLibrary:game:status', (event, id, status /* , error */) => {
      store.commit('setGameStatus', { id, status });
    });

    // Game download progress updated
    ipcRenderer.on('gameLibrary:game:download-progress', (event, id, progress) => {
      store.commit('setGameDownloadProgress', { id, progress });
    });
  },

  /**
   * Installs game by it's ID.
   *
   * @param {Vuex.Store} store Vuex store
   * @param {number} id Installed game ID
   * @returns {?Promise} Promise that resolves when game is successfully installed.
   *                     Rejects if something went wrong.
   */
  installGame(store, id) {
    if (!ipcRenderer) throw new Error('Manipulating games is available only in desktop client');
    const status = store.getters.getGameStatus(id);
    // This should be handled within components, but check it to ensure
    if (status === GAME_STATUS.INSTALLED) return;
    try {
      // Set game's state as downloading.
      store.commit('setGameStatus', { id, status: GAME_STATUS.DOWNLOADING });
      // Wrap download method to try catch, because download errors are using Promise#reject
      try {
        // Actually start downloading file with main process.
        // Main process also should start updating installation progress.
        ipcRenderer.send('gameLibrary:game:install', id);
      } catch (err) {
        // TODO handle it somehow
      }
    } catch (err) {
      // An error always should mean that something went wrong and game was removed
      store.commit('setGameStatus', { id, status: GAME_STATUS.UNINSTALLED });
      // TODO error message
    }
  },

  /**
   * Uninstalls game by it's ID
   *
   * @param {Vuex.Store} store Vuex store
   * @param {any} id Uninstalled game ID
   * @returns {?Promise} Promise that resolves when game is successfully uninstalled.
   *                     Rejects if something went wrong.
   */
  async uninstallGame(store, id) {
    if (!ipcRenderer) throw new Error('Manipulating games is available only in desktop client');
    // This should be handled within components, but check it to ensure
    if (status !== GAME_STATUS.UNINSTALLED) return null;
    // Remove file with main process
    ipcRenderer.send('gameLibrary:game:uninstall', id);
    // eslint-disable-next-line promise/avoid-new
    return new Promise((resolve, reject) => {
      const unsubscribe = ipcRenderer.on('gameLibrary:game:status', (updateId, updateStatus) => {
        // Make sure that this event is about current game
        if (id === updateId) {
          // Anyway we don't need this listener anymore
          unsubscribe();
          // If game status is uninstalled everything went good
          // Otherwise we might want to show error
          if (updateStatus === GAME_STATUS.UNINSTALLED) resolve(); else reject();
        }
      });
    });
  },
};

export default {
  state: initialState,
  getters,
  actions,
  mutations,
};
