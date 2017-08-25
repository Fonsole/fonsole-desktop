/**
 * @file A module to manipulate local games.
 *       This module will be loaded only on electron version.
 */

import { ipcRenderer } from 'electron';

const initialState = {
  cachedGames: [
    {
      path: 'd:/dev/fonsole/games/game1',

      /* Fetched from workshop */
      name: 'Game 1',
      tags: [
        'strategy',
        'action',
      ],
      // Not sure that workshop can handle it.
      // If it cant's => put this to .fonsole.yml
      players: [3, 6],
      // Undefined if not published
      workshopId: 1234567,
    },
    {
      path: 'd:/dev/fonsole/games/game2',
      name: 'Game 2',
      tags: [
        'strategy',
        'action',
      ],
      players: [3, 6],
      workshopId: undefined,
    },
  ],
  gameUpdateListeners: {},
};

const initialGetters = {
  // Game paths are stored in settings, so just return value from settings module
  workshopGamePaths: (state, getters, rootState) => rootState.settings.workshopGames || [],
  // Either return already fetched information, or run fetching process
  workshopGameInfo: state => (gamePath) => {
    const cachedGame = state.cachedGames.find(game => game.path === gamePath);
    if (cachedGame) return cachedGame;
    ipcRenderer.send('workshop:game:fetch', gamePath);
    if (!state.fetchGameInfo[gamePath]) state.fetchGameInfo[gamePath] = [];
    // eslint-disable-next-line promise/avoid-new
    return new Promise((resolve) => {
      state.fetchGameInfo[gamePath].push(() => {
        // TODO
        resolve();
      });
    });
  },
};

const mutations = {};

const actions = {
  /**
   * Adds existing game to list.
   *
   * @param {Vuex.Store} store Vuex store
   * @param {string} gamePath New game path
   * @returns {?Promise} Promise that resolves when game is successfully linked.
   *                     Rejects if something went wrong.
   */
  addGame(store, gamePath) {
    ipcRenderer.emit('workshop:game:add', gamePath);
  },

  /**
   * Removes game.
   *
   * @param {Vuex.Store} store Vuex store
   * @param {object} payload Action payload
   * @param {string} payload.gamePath Uninstalled game path
   * @param {?boolean} payload.removeFiles If true removes game files,
   *                                       otherwise removes only link to game.
   * @returns {?Promise} Promise that resolves when game is successfully removed.
   *                     Rejects if something went wrong.
   */
  removeGame(store, payload) {
    ipcRenderer.emit('workshop:game:remove', payload.gamePath, payload.removeFiles);
  },
};

export default {
  state: initialState,
  getters: initialGetters,
  actions,
  mutations,
};
