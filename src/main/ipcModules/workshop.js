import { ipcMain } from 'electron';

/**
 * Tries to get all information about local game.
 *
 */
function fetchGameInfo() {

}

/**
 * Hooks to IPC events
 */
export default () => {
  ipcMain.on('workshop:game:fetch', fetchGameInfo);
};
