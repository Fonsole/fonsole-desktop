/**
 * @file All enums shared between Main and Renderer processes
 */

/**
 * All possible game statuses
 *
 * @readonly
 * @enum {string}
 */
export const GAME_STATUS = { // eslint-disable-line import/prefer-default-export
  UNINSTALLED: 0,
  INSTALLED: 1,
  UPDATABLE: 2,
  DOWNLOADING: 3,
};
