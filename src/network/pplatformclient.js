// pure platform interface

import Netgroup from './netgroupclient';
import SignalingMessageType from './SignalingMessageTypes';
import TAG from './tags';

/*
Unused on client side
class ControllerDiscoveryMessage {
  constructor(lConnectionId, lUserId, lName) {
    this.connectionId = lConnectionId;
    this.userId = lUserId;
    this.name = lName;
  }
}
*/

class Controller {
  constructor(lConnectionId, lUserId, lName) {
    this.connectionId = lConnectionId;
    this.userId = lUserId;
    this.name = lName;
  }

  getName() {
    return this.name;
  }

  getId() {
    return this.userId;
  }

  toString() {
    return `Controller[connectionId: ${this.connectionId}, userId:${this.userId}, , name:${name}`;
  }
}

class PPlatform {
  constructor() {
    this.mIsConnected = false;
    this.VIEW_USER_ID = 0;
    this.HOST_CONTROLLER_USER_ID = 1;

    this.mActiveGame = null;

    this.mIsHostController = false;

    this.mMessageListener = [];

    this.mOwnUserId = -1;

    // This value will be null until a view was discovered.
    // Only then a controller is fully initialized
    this.mViewId = null;

    // only available on the view side for now
    this.mControllers = {};

    // the name the user chose.
    this.mOwnName = null;

    this.sigChan = new Netgroup();
  }

  isConnected() {
    return this.mIsConnected;
  }

  /**
     * Returns an object containing connection id as key and controller
     * information as values.
     *
     * @returns {c|PPlatform.mControllers}
     */
  getControllers() {
    return this.mControllers;
  }

  Log(msg) { // eslint-disable-line
    console.debug(msg);
  }

  isHostController() {
    return this.mIsHostController;
  }
  getOwnUserId() {
    return this.mOwnUserId;
  }
  /**
   * Legacy version of getOwnUserId
   *
   * @returns {?number} Own user ID
   */
  getOwnId() {
    return this.mOwnUserId;
  }
  getOwnConnectionId() {
    return this.sigChan.getOwnId();
  }

  enterGame(lGame) {
    this.mActiveGame = lGame;
    this.sendMessage(TAG.ENTER_GAME, lGame);
  }

  join(key, name) {
    this.mOwnName = name;
    this.sigChan.open(key, this.OnNetgroupMessageInternal.bind(this));
    this.mIsHostController = true;
  }

  disconnect() {
    this.sigChan.close();
  }
  sendMessageObj(lTag, lObj, lTo) {
    this.sendMessage(lTag, JSON.stringify(lObj), lTo);
  }
  sendMessage(lTag, lContent, lTo) {
    // the content is in typesafe platforms defined as "string"
    // it should always be a string even if it is empty
    if (typeof lContent === 'undefined' || lContent == null) {
      lContent = '';
    }
    const msg = {};
    msg.tag = lTag;
    msg.content = lContent;

    this.Log(`Snd: TAG: ${lTag} data: ${JSON.stringify(msg)} to ${lTo}`);
    this.sigChan.sendMessageTo(JSON.stringify(msg), lTo);
  }
  addMessageListener(lListener) {
    this.mMessageListener.push(lListener);
  }

  /**
   * Will handle all incomming messages + send them to the listener outside of pplatform
   *
   * All tasks that are unique to the netgroup commands don't belong here
   */
  handleMessage(lTag, lContent, lId) {
    // events to handle before the content gets it

    switch (lTag) {
      case TAG.ENTER_GAME: {
        this.ShowGame(lContent);
        break;
      }
      case TAG.SERVER_FULL: {
        this.disconnect();
        break;
      }
      case TAG.NAME_IN_USE: {
        this.disconnect();
        break;
      }
      case TAG.CONTROLLER_DISCOVERY: {
        const controllerDiscoveryData = JSON.parse(lContent);
        const c = new Controller(
          controllerDiscoveryData.connectionId,
          controllerDiscoveryData.userId,
          controllerDiscoveryData.name,
        );
        this.mControllers[controllerDiscoveryData.userId] = c;
        if (this.getOwnConnectionId() === controllerDiscoveryData.connectionId) {
          this.mOwnName = controllerDiscoveryData.name;
          this.mOwnUserId = controllerDiscoveryData.userId;
        }
        break;
      }
      case TAG.VIEW_DISCOVERY: {
        this.mViewId = lId; // store the id for later

        let name = this.mOwnName;
        if (name == null || name === '') {
          name = ''; // empty name will be replaced by the server
        }
        // register as controller at the view
        const controllerRegisterData = { name };
        this.sendMessageObj(TAG.CONTROLLER_REGISTER, controllerRegisterData, this.mViewId);
        break;
      }
      case TAG.CONTROLLER_LEFT: {
        delete this.mControllers[lId];
      }
    }

    // send the event out to the game and ui
    for (let i = 0; i < this.mMessageListener.length; i += 1) {
      this.mMessageListener[i](lTag, lContent, lId);
    }
  }

  /**
   * This is the message handler based on Netgroup.
   * Only the content ofer user messages will be send
   * to the games outside of platform via mMessageListener
   */
  OnNetgroupMessageInternal(lType, lId, lMsg) {
    switch (lType) {
      case SignalingMessageType.Connected: {
        this.Log('Connected');
        this.mIsConnected = true;
        this.onConnect();
        // ShowGame("gamelist");
        break;
      }
      case SignalingMessageType.UserMessage: {
        const msgObj = JSON.parse(lMsg);

        this.Log(`Rec: TAG: ${msgObj.tag} data:${JSON.stringify(msgObj.content)} from ${lId}`);
        this.handleMessage(msgObj.tag, msgObj.content, lId);
        break;
      }
      case SignalingMessageType.Closed: {
        this.mIsConnected = false;
        this.onClose();
        this.Log('Disconnected');
        break;
      }
      case SignalingMessageType.UserJoined: {
        this.Log(`User ${lId} joined`);


        // controller ignore these so far
        break;
      }
      case SignalingMessageType.UserLeft: {
        this.Log(`User ${lId} left`);

        // controller ignore these so far
        break;
      }
      default: {
        this.Log(`Invalid message received. type: ${lType} content: ${lMsg}`);
      }
    }
  }

  injectAPI(obj) {
    obj.platformType = 'desktop';
    obj.platformSendMessage = this.sendMessage.bind(this);
    obj.platformAddMessageListener = this.addMessageListener.bind(this);
  }

  // Private

  onClose() {
    this.handleMessage(TAG.DISCONNECTED, null, -1);
    // disconnected. clean up all data
    this.mControllers = {};
  }

  onConnect() { // eslint-disable-line

  }

  ShowGame(lGameName) {
    this.mActiveGame = lGameName;
    this.Vue.$store.commit('setGame', lGameName);
  }

  static GetRandomKey() {
    let result = '';
    for (let i = 0; i < 6; i += 1) {
      result += String.fromCharCode(65 + Math.round(Math.random() * 25));
    }
    return result;
  }
}

export {
  Controller,
  PPlatform,
};
