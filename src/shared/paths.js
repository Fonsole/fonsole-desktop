import { app } from 'electron';

export const USER_DATA = app.getPath('userData');
export const SETTINGS_PATH = `${USER_DATA}/settings.yml`;
export const PATH_LOCAL_GAMES = `${USER_DATA}/games/`;

export const getGameDownloadUrl = id => `https://github.com/electron/electron/releases/download/v1.6.11/electron-v1.6.11-win32-x64.zip?id=${id}`;
export const getGameArchivePath = id => `${PATH_LOCAL_GAMES}/game-${id}.zip`;
export const getGameInstallPath = id => `${PATH_LOCAL_GAMES}/${id}`;
