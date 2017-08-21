/**
 * @file All enums shared between Main and Renderer processes
 */

/**
 * All possible game statuses
 *
 * @readonly
 * @enum {string}
 */
export const GAME_STATUS = {
  UNINSTALLED: 0,
  INSTALLED: 1,
  UPDATABLE: 2,
  DOWNLOADING: 3,
};


/**
 * Types of workshop preview window.
 * Also can be a string with game path.
 *
 * @readonly
 * @enum {string}
 */
export const WORKSHOP_PREVIEW_TYPE = {
  PLACEHOLDER: 0,
  CREATE: 1,
};
