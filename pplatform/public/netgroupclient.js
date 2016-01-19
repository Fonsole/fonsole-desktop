//Copyright (C) 2015 Christoph Kutza

    //maps the string based messages of mSocketio to clear id based messages
    var SignalingMessageType = {
        Invalid : 0,
        Connected : 1,
        Closed : 2,
        UserMessage : 3,
        UserJoined : 4,
        UserLeft : 5
    };
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
        }
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
            var lMsgObj = {};
            lMsgObj.id = lToUserId;
            lMsgObj.content = lContent;
            mSocket.emit('user message', JSON.stringify(lMsgObj));
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
            OnClose();
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
                OnClose();
            });
            mSocket.on('connect_error', function() {
                OnClose();
            });
            mSocket.on('disconnect', function() {
                OnClose();
            });
            
            mSocket.on('connect', function() {
                if(mRoomOwner){
                    mSocket.emit('open room', mRoomName);
                }
                else{
                    mSocket.emit('join room', mRoomName);
                }
            });
            
            //custom messages
            mSocket.on("room opened", function(lMsg) {
                var lMsgObj = JSON.parse(lMsg);
                OnConnect(lMsgObj.name, lMsgObj.id);
            });
            mSocket.on("room joined", function(lMsg) {
                var lMsgObj = JSON.parse(lMsg);
                OnConnect(lMsgObj.name, lMsgObj.id);
            });
            
            mSocket.on("user message", function(lMsg) {
                var lMsgObj = JSON.parse(lMsg);
                OnUserMessage(lMsgObj.id, lMsgObj.content);
            });
            mSocket.on("user joined", function(lMsg) {
                OnUserJoined(lMsg);
            });
            mSocket.on("user left", function(lMsg) {
                OnUserLeft(lMsg);
            });
        }
        
        /**
         * Adds handlers to the mSocket
         */
        function SetupmSocket()
        {
            
        }
        
        
 //event handlers
        /**\
         * Called after open or join call was successful
         * 
         */
        function OnConnect(lName, lId)
        {
            mRoomName = lName;
            mOwnId = lId;
            mConnecting = false;
            mConnected = true;
            mHandler(SignalingMessageType.Connected, mOwnId, null);
        }
        function OnUserJoined(lId)
        {
            mHandler(SignalingMessageType.UserJoined, lId, null);
        }
        function OnUserLeft(lId)
        {
            mHandler(SignalingMessageType.UserLeft, lId, null);
        }
        function OnUserMessage(lId, lContent)
        {
            mHandler(SignalingMessageType.UserMessage, lId, lContent);
        }
        /**
         * Sends out close event if the system was connecting or connected in the first place.
         * 
         * Note: This might also be called because of a timeout
         */
        function OnClose()
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