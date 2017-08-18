/**
 * @file A module to manipulate local games.
 */

// @ifdef ELECTRON
import { ipcRenderer } from 'electron';
// @endif

const initialState = {
  games: [
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

const getters = {
  workshopGamePaths: state => state.games.map(game => game.path),
  workshopGameInfo: state => (gamePath) => {
    if (!ipcRenderer) throw new Error('Workshop is available only in desktop client');
    const cachedGame = state.games.find(game => game.path === gamePath);
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
   * Adds existing game to list
   *
   * @param {Vuex.Store} store Vuex store
   * @param {string} gamePath New game path
   * @returns {?Promise} Promise that resolves when game is successfully linked.
   *                     Rejects if something went wrong.
   */
  linkGame(/* store */) {
    if (!ipcRenderer) throw new Error('Workshop is available only in desktop client');
  },

  /**
   * Removes game
   *
   * @param {Vuex.Store} store Vuex store
   * @param {string} gamePath Uninstalled game path
   * @returns {?Promise} Promise that resolves when game is successfully removed.
   *                     Rejects if something went wrong.
   */
  removeGame(/* store */) {
    if (!ipcRenderer) throw new Error('Workshop is available only in desktop client');
  },
};

export default {
  state: initialState,
  getters,
  actions,
  mutations,
};
