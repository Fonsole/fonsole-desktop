//Copyright (C) 2015 Christoph Kutza

    var EVENT_CONNECTION = "connection";
    var MESSAGE_NAME = "SMSG";
    var SignalingMessageType = {
        Invalid : 0,
        Connected : 1,
        Closed : 2,
        UserMessage : 3,
        UserJoined : 4,
        UserLeft : 5,
        OpenRoom : 6,
        JoinRoom : 7
    };
    
    
    function SMessage(lMsgType, lMsgContent, lUserId)
    {
        this.type = lMsgType;
        this.content = lMsgContent;
        this.id = lUserId;
    }
    
    function Netgroup(lUrl)
    {
        //socket.io object to access the network
        var mSocket;
        
        //true if the object is setup to own the room (or tring to own it during connecting/opening process)
        var mRoomOwner = false;
        
        //roomname
        var mRoomName = null;
        
        //in process of connecting / joining or opening a room
        var mConnecting = false;
        
        //connection etablished and room joined/opened
        var mConnected = false;
        
        //own id or null if no id as not yet connected
        var mOwnId = null;
        this.getOwnId = function()
        {
            return mOwnId;
        };
        //handler the messages are delivered to (set during opening/connecting)
        var mHandler = null;
        
        //url socket.io uses to connect (using this default if not set in constructor)
        var mUrl = "http://localhost:3001";
        if(lUrl != null)
            mUrl = lUrl;
        
        var self = this;
        /**Opens a new room. Result will be:
        * * SignalingMessageType.Connected if the room is opened
        * * SignalingMessageType.Closed if the room is already opened
        * 
        * @param {type} lName
        * @param {type} lHandler a function(SignalingMessageType, UserId or null, content or null
        * @returns {undefined}
        */
        this.open = function(lName, lHandler)
        {
            mHandler = lHandler;
            mRoomName = lName;
            mConnecting = true;
            mRoomOwner = true;
            
            CreatemSocket();
        };
        
        /**Same as open but it only connects to an existing room
        * 
        * @param {type} lName
        * @param {type} lHandler
        * @returns {undefined}
        */
        this.connect = function(lName, lHandler)
        {
            mHandler = lHandler;
            mRoomName = lName;
            mConnecting = true;
            mRoomOwner = false;
            
            CreatemSocket();
        };
        
        this.sendMessageTo = function(lContent, lToUserId)
        {
            var lId = -1;
            
            //filter out "undefined" and strings to enforce type
            //savety. This helps to not confuse the C# or any other typesafe
            //versions
            if(typeof lToUserId === "number")
                lId = lToUserId;
            
            
            var lMsgObj = new SMessage(SignalingMessageType.UserMessage, lContent, lId);
            mSocket.emit(MESSAGE_NAME, lMsgObj);
        };
        /**Sends a message over the signaling channel.
        * This can either be a broadcast to everyone in the room or be done more
        * securely by allowing only someone who connects to send to the server
        * and the other way around. The used protocol can work with both.
        * 
        * @param {type} lContent
        * @returns {undefined}
        */
        this.sendMessage = function(lContent)
        {
            self.sendMessageTo(lContent);
        };
        
        /**Closes the signaling channel
        * 
        * @returns {undefined}
        */
        this.close = function()
        {
            mSocket.disconnect();
            onClose();
        };
        
        
//private methods / helpers
        /**
         * Creates and configurates the mSocket used
         */
        function CreatemSocket()
        {
            mSocket = io.connect(mUrl, {'force new connection': true, reconnection : false, timeout : 5000});
            
            
            
            //mSocket.io special messages
            mSocket.on('connect_timeout', function() {
                onClose();
            });
            mSocket.on('connect_error', function() {
                onClose();
            });
            mSocket.on('disconnect', function() {
                onClose();
            });
            
            mSocket.on('connect', function() {
                if(mRoomOwner){
                    var lMsgObj = new SMessage(SignalingMessageType.OpenRoom, mRoomName, -1);
                    mSocket.emit(MESSAGE_NAME, lMsgObj);
                }
                else{
                    var lMsgObj = new SMessage(SignalingMessageType.JoinRoom, mRoomName, -1);
                    mSocket.emit(MESSAGE_NAME, lMsgObj);
                }
            });
            
            //custom messages
            mSocket.on(MESSAGE_NAME, function(lMsgObj) {
                console.log("REC: " + JSON.stringify(lMsgObj));
                if(lMsgObj.type == SignalingMessageType.Connected)
                {
                    onConnect(lMsgObj.content, lMsgObj.id);
                }else if(lMsgObj.type == SignalingMessageType.Connected)
                {
                    onConnect(lMsgObj.content, lMsgObj.id);
                }else if(lMsgObj.type == SignalingMessageType.UserMessage)
                {
                    onUserMessage(lMsgObj.id, lMsgObj.content);
                }else if(lMsgObj.type == SignalingMessageType.UserLeft)
                {
                    onUserJoined(lMsgObj.id);
                }else if(lMsgObj.type == SignalingMessageType.UserLeft)
                {
                    onUserLeft(lMsgObj.id);
                }
            });
            
        }
        
        
 //event handlers
 
        function onConnect(lName, lId)
        {
            mRoomName = lName;
            mOwnId = lId;
            mConnecting = false;
            mConnected = true;
            mHandler(SignalingMessageType.Connected, mOwnId, null);
        }
        function onUserJoined(lId)
        {
            mHandler(SignalingMessageType.UserJoined, lId, null);
        }
        function onUserLeft(lId)
        {
            mHandler(SignalingMessageType.UserLeft, lId, null);
        }
        function onUserMessage(lId, lContent)
        {
            mHandler(SignalingMessageType.UserMessage, lId, lContent);
        }
        /**
         * Sends out close event if the system was connecting or connected in the first place.
         * 
         * Note: This might also be called because of a timeout
         */
        function onClose()
        {
            if(mConnecting || mConnected)
            {
                //only send out the event if the user wasn't informed yet
                //if both values are false the user either did never connect or
                //did call Close already and thus received a closed event already
                mHandler(SignalingMessageType.Closed, null, null);
            }
            mConnecting = false;
            mConnected = false;
        }
    } //class ends