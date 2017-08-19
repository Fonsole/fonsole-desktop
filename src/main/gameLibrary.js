import request from 'request';
import progress from 'request-progress';
import fs from 'fs-extra';
import { ipcMain } from 'electron';
import { GAME_STATUS } from '=/enums';
import Promise from 'bluebird';
import extract from 'extract-zip';
import {
  PATH_LOCAL_GAMES,
  getGameDownloadUrl,
  getGameArchivePath,
  getGameInstallPath,
} from '=/paths';

const extractPromise = Promise.promisify(extract);

/**
 * Starts game loading event
 * Initially synchronously returns all installed games.
 * After that scans all games and checks them by hash for updates
 *
 * @todo Check for updates and verify games hashes
 * @todo Watch for changes
 * @param {object} event Electron IPC event
 */
function loadGames(event) {
  // Ensure that games directory exists
  fs.ensureDirSync(PATH_LOCAL_GAMES);
  // Get all files from fonsole directory.
  const files = fs.readdirSync(PATH_LOCAL_GAMES);
  // Filter them, so we'll get only directories
  const directories = files.filter(file => fs.statSync(`${PATH_LOCAL_GAMES}/${file}`).isDirectory());
  // Get only valid Game ID's from all directories
  const gameIds = directories.map(id => +id).filter(id => !isNaN(id));
  // Return it to event, so players can see games immediate.
  event.returnValue = gameIds;
}

/**
 * Download's game's zip archive and extracts it.
 *
 * @todo Handle errors
 * @param {object} event Electron IPC event
 * @param {Number} gameId Required game ID
 */
function install(event, gameId) {
  // Wrap everything with try, so Vue can handle all other errors
  try {
    const downloadPath = getGameArchivePath(gameId);
    const unpackPath = getGameInstallPath(gameId);
    // Create a request to download game archive
    // TODO Stub link for now
    progress(request(getGameDownloadUrl(gameId)))
      .on('progress', (state) => {
        // Send download progress to renderer, so players can track it
        event.sender.send('gameLibrary:game:download-progress', gameId, state);
      })
      .on('error', async (error) => {
        // Cleanup
        fs.remove(downloadPath);
        // Notify player about downloading exception
        event.sender.send('gameLibrary:game:status', GAME_STATUS.UNINSTALLED, error);
      })
      .on('end', async () => {
        // Set finished progress state
        event.sender.send('gameLibrary:game:download-progress', gameId, { unpacking: true });
        try {
          // Try to extract downloaded archive
          await extractPromise(downloadPath, { dir: unpackPath });
          // Reset download progress
          event.sender.send('gameLibrary:game:download-progress', gameId, null);
          // Update status to installed
          event.sender.send('gameLibrary:game:status', GAME_STATUS.INSTALLED);
        } catch (err) {
          // Something went wrong with archive extraction.
          fs.remove(downloadPath);
          fs.remove(unpackPath);
          // Game wasn't installed
          event.sender.send('gameLibrary:game:status', GAME_STATUS.UNINSTALLED, err);
        }
      })
      .pipe(fs.createWriteStream(downloadPath));
  } catch (err) {
    // Game wasn't installed for some uncaught reason
    // (most likely something prevents creation of write stream)
    event.sender.send('gameLibrary:game:status', GAME_STATUS.UNINSTALLED, err);
  }
}

/**
 * Removes all files related to game
 *
 * @param {object} event Electron IPC event
 * @param {any} id Game ID
 */
function uninstall(event, id) {
  // Remove game archive in a case of failed download
  fs.remove(getGameArchivePath(id));
  // Remove unpacked game directory
  fs.remove(getGameInstallPath(id));
  // Update status on renderer
  event.sender.send('gameLibrary:game:status', GAME_STATUS.UNINSTALLED);
}

/**
 * Hooks to IPC events
 */
export default () => {
  ipcMain.on('gameLibrary:library:load', loadGames);
  ipcMain.on('gameLibrary:game:uninstall', uninstall);
  ipcMain.on('gameLibrary:game:install', install);
};
