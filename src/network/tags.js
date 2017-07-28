export default {
  CONTROLLER_REGISTER: 'PLATFORM_CONTROLLER_REGISTER',
  CONTROLLER_DISCOVERY: 'PLATFORM_CONTROLLER_DISCOVERY',


  CONTROLLER_LEFT: 'PLATFORM_CONTROLLER_LEFT',
  VIEW_DISCOVERY: 'PLATFORM_VIEW_DISCOVERY',
  ENTER_GAME: 'PLATFORM_ENTER_GAME',
  SERVER_FULL: 'PLATFORM_SERVER_FULL',
  NAME_IN_USE: 'PLATFORM_NAME_IN_USE',
  ROOM_LOCKED: 'PLATFORM_ROOM_LOCKED',

  // special message to inform the games and other things connected to the platform
  // that the connection was disconnected. current ui will reload the page
  // in the future this could be used to show errors
  DISCONNECTED: 'PLATFORM_DISCONNECTED',
};
