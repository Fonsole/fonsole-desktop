import fs from 'fs-extra';
import yaml from 'js-yaml';
import { ipcMain } from 'electron';
import { verifyGameIndex } from '=/util';

/**
 * Tries to get all information about local game.
 *
 * @param event Ipc event
 */
async function fetchGameInfo({ sender }, gamePath) {
  // Make sure that game directory exists
  if (fs.existsSync(gamePath)) {
    // All games should have .fonsole.yml index file in their root
    const fonsoleYmlPath = `${gamePath}/.fonsole.yml`;
    if (fs.existsSync(fonsoleYmlPath)) {
      const content = await fs.readFile(fonsoleYmlPath, 'utf8');
      const index = yaml.safeLoad(content);
      if (verifyGameIndex(index)) {
        console.log(`"${gamePath}" - ok`);
      }
    }
  }
  fs.readFileSync('');
  sender.send('ok', 1);
}

/**
 * Hooks to IPC events
 */
export default () => {
  ipcMain.on('workshop:game:fetch', fetchGameInfo);
};
