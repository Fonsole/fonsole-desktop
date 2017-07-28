// Copyright (C) 2015 Christoph Kutza

import io from 'socket.io-client';
import SignalingMessageType from './SignalingMessageTypes';

// Unused
// const EVENT_CONNECTION = 'connection';

const MESSAGE_NAME = 'SMSG';

class SMessage {
  constructor(lMsgType, lMsgContent, lUserId) {
    this.type = lMsgType;
    this.content = lMsgContent;
    this.id = lUserId;
  }
}

export default class Netgroup {
  constructor(lUrl = (`${window.location.protocol}//${window.location.hostname}:3001`)) {
    // socket.io server url
    this.mUrl = lUrl;

    // socket.io object to access the network
    this.mSocket = null;

    // true if the object is setup to own the room
    // (or tring to own it during connecting/opening process)
    this.mRoomOwner = false;

    // roomname
    this.mRoomName = null;

    // in process of connecting / joining or opening a room
    this.mConnecting = false;

    // connection etablished and room joined/opened
    this.mConnected = false;

    // handler the messages are delivered to (set during opening/connecting)
    this.mHandler = null;

    // own id or null if no id as not yet connected
    this.mOwnId = null;
  }

  /**
   * Returns own id
   *
   * @returns {?number} Own ID. Can be null if no id as not yet connected
   * @memberof Netgroup
   */
  getOwnId() {
    return this.mOwnId;
  }

  /**
   * Opens a new room. Result will be:
   * * SignalingMessageType.Connected if the room is opened
   * * SignalingMessageType.Closed if the room is already opened
   *
   * @param {type} lName Room name
   * @param {type} lHandler a function(SignalingMessageType, UserId or null, content or null
   * @returns {void}
   */
  open(lName, lHandler) {
    this.mHandler = lHandler;
    this.mRoomName = lName;
    this.mConnecting = true;
    this.mRoomOwner = true;

    this.CreatemSocket();
  }

  /**
   * Same as open but it only connects to an existing room
   *
   * @param {type} lName Room name
   * @param {type} lHandler a function(SignalingMessageType, ?UserId, ?content)
   * @returns {void}
   */
  connect(lName, lHandler) {
    this.mHandler = lHandler;
    this.mRoomName = lName;
    this.mConnecting = true;
    this.mRoomOwner = false;

    this.CreatemSocket();
  }

  /**
   * Creates and configurates the mSocket used
   *
   * @returns {void}
   */
  CreatemSocket() {
    const mSocket = io.connect(this.mUrl, {
      'force new connection': true,
      reconnection: false,
      timeout: 5000,
    });
    this.mSocket = mSocket;

    // mSocket.io special messages
    mSocket.on('connect_timeout', () => {
      this.onClose();
    });
    mSocket.on('connect_error', () => {
      this.onClose();
    });
    mSocket.on('disconnect', () => {
      this.onClose();
    });

    mSocket.on('connect', () => {
      if (this.mRoomOwner) {
        const lMsgObj = new SMessage(SignalingMessageType.OpenRoom, this.mRoomName, -1);
        mSocket.emit(MESSAGE_NAME, lMsgObj);
      } else {
        const lMsgObj = new SMessage(SignalingMessageType.JoinRoom, this.mRoomName, -1);
        mSocket.emit(MESSAGE_NAME, lMsgObj);
      }
    });

    // custom messages
    mSocket.on(MESSAGE_NAME, (lMsgObj) => {
      console.log(`REC: ${JSON.stringify(lMsgObj)}`);
      switch (lMsgObj.type) {
        case SignalingMessageType.Connected: {
          this.onConnect(lMsgObj.content, lMsgObj.id);
          break;
        }
        case SignalingMessageType.UserMessage: {
          this.onUserMessage(lMsgObj.id, lMsgObj.content);
          break;
        }
        case SignalingMessageType.UserJoined: {
          this.onUserJoined(lMsgObj.id);
          break;
        }
        case SignalingMessageType.UserLeft: {
          this.onUserLeft(lMsgObj.id);
          break;
        }
        default: {
          console.error('Unexpected socket message type');
        }
      }
    });
  }

  sendMessageTo(lContent, lToUserId = -1) {
    // filter out "undefined" and strings to enforce type savety.

    // eslint-disable-next-line no-param-reassign
    if (typeof lToUserId !== 'number') { lToUserId = -1; }

    const lMsgObj = new SMessage(SignalingMessageType.UserMessage, lContent, lToUserId);
    this.mSocket.emit(MESSAGE_NAME, lMsgObj);
  }

  /**
   * Sends a message over the signaling channel.
   * This can either be a broadcast to everyone in the room or be done more
   * securely by allowing only someone who connects to send to the server
   * and the other way around. The used protocol can work with both.
   *
   * @param {type} lContent Message content
   * @returns {void}
   */
  sendMessage(lContent) {
    this.sendMessageTo(lContent);
  }

  /**
     * Closes the signaling channel
     *
     * @returns {void}
     */
  close() {
    this.mSocket.disconnect();
    this.onClose();
  }

  // Event handlers
  /**
   * An event that occours when client connects to socket
   *
   * @param {string} lName Room name
   * @param {any} lId Own ID
   * @returns {void}
   */
  onConnect(lName, lId) {
    this.mRoomName = lName;
    this.mOwnId = lId;
    this.mConnecting = false;
    this.mConnected = true;
    this.mHandler(SignalingMessageType.Connected, this.mOwnId, null);
  }

  /**
   * An event that occours when client joins to room
   *
   * @param {any} lId Connected user ID
   * @returns {void}
   */
  onUserJoined(lId) {
    this.mHandler(SignalingMessageType.UserJoined, lId, null);
  }
  onUserLeft(lId) {
    this.mHandler(SignalingMessageType.UserLeft, lId, null);
  }
  onUserMessage(lId, lContent) {
    this.mHandler(SignalingMessageType.UserMessage, lId, lContent);
  }

  /**
   * Sends out close event if the system was connecting or connected in the first place.
   *
   * Note: This might also be called because of a timeout
   *
   * @returns {void}
   */
  onClose() {
    if (this.mConnecting || this.mConnected) {
      this.mConnecting = false;
      this.mConnected = false;
      // only send out the event if the user wasn't informed yet
      // if both values are false the user either did never connect or
      // did call Close already and thus received a closed event already
      this.mHandler(SignalingMessageType.Closed, null, null);
    }
  }
}
