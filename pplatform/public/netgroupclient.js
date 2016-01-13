//Copyright (C) 2015 Christoph Kutza

 function Netgroup(lUrl)
    {
        var socket;
        var mHandler = null;
        var mConnecting = false;
        var mConnected = false;
        
        var mUrl = "http://localhost:3001";
        if(lUrl != null)
            mUrl = lUrl;
        
        /**Opens a new room. Result will be:
        * * SignalingMessageType.Connected if the room is opened
        * * SignalingMessageType.Closed if the room is already opened
        * 
        * @param {type} lName
        * @param {type} lHandler a function(SignalingMessageType, messageContent(string));
        * @returns {undefined}
        */
        this.open = function(lName, lHandler)
        {
            mHandler = lHandler;
            mConnecting = true;
            
            socket = CreateSocket();
            socket.on('connect', function() {
                socket.emit('open room', lName);
            });
            socket.on("room opened", function() {
                OnConnect();
            });
            SetupSocket();
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
            mConnecting = true;
            socket = CreateSocket();
            socket.on('connect', function() {
                socket.emit('join room', lName);
            });
            socket.on("room joined", function() {
                OnConnect();
            });
            SetupSocket();
        };
        
        function CreateSocket()
        {
            return io.connect(mUrl, {'force new connection': true, reconnection : false, timeout : 5000});
        }
        
        function SetupSocket()
        {
            socket.on('connect_timeout', function() {
                OnClose();
            });
            socket.on('connect_error', function() {
                OnClose();
            });
            socket.on('disconnect', function() {
                OnClose();
            });
            socket.on("msg", function(msg) {
                mHandler(SignalingMessageType.UserMessage, msg);
            });
        }
        
        /**Closes the signaling channel
        * 
        * @returns {undefined}
        */
        this.Close = function()
        {
            socket.disconnect();
            OnClose();
        };
        
        
        function OnConnect()
        {
            mConnecting = false;
            mConnected = true;
            mHandler(SignalingMessageType.Connected, null);
        }
        /**
         * Sends out close event if the system was connecting or connected in the first place.
         * 
         * TODO: This might also be called because of a timeout
         */
        function OnClose()
        {
            if(mConnecting || mConnected)
            {
                //only send out the event if the user wasn't informed yet
                //if both values are false the user either did never connect or
                //did call Close already and thus received a closed event already
                mHandler(SignalingMessageType.Closed, null);
            }
            mConnecting = false;
            mConnected = false;
        }
        
        /**Sends a message over the signaling channel.
        * This can either be a broadcast to everyone in the room or be done more
        * securely by allowing only someone who connects to send to the server
        * and the other way around. The used protocol can work with both.
        * 
        * @param {type} lMessage
        * @returns {undefined}
        */
        this.sendMessage = function(lMessage)
        {
            socket.emit('msg', lMessage);
        };
    }