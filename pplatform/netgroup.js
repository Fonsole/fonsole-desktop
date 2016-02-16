





module.exports = function()
{
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
    
    var Server = require('socket.io');
    var io;
    
        
    //counter for connections (not concurrent!)
    var connectionCount = 0;

    //here we keep track of the rooms
    var gRooms = {};
    
    
    
    this.listen = function(lHttpOrPort)
    {
        io = new Server(lHttpOrPort);
        
        
        //wait for connections via socket.io. if a connection is etablished setup all the handlers for messages
        io.on(EVENT_CONNECTION, function(lSocket) {
            
            connectionCount++;
            console.log('a user ' + connectionCount + ' connected');
            var connection = new Connection(connectionCount);
            connection.init(lSocket);
        });
    };
    
    
    function Room(lName)
    {
        var mName = lName;
        var mConnection = {};
        
        //add to the global room list
        gRooms[mName] = this; 
        
        this.getName = function()
        {
            return mName;
        };
        this.addConnection = function(lConnection)
        {
            sendUserJoined(lConnection);
            mConnection[lConnection.getId()] = lConnection;
        }
        
        this.remConnection = function(lConnection)
        {
            console.log('send user left message');
            delete mConnection[lConnection.getId()];
            sendUserLeft(lConnection);
        }
        
        this.close = function()
        {    
            for(var id in mConnection)
            {
                mConnection[id].disconnect();
                console.log('room ' + mName + ' closing. Disconnected user ' + id);
            }
            
            //remove from roomlist
            delete gRooms[mName];
            
                    
            console.log('Room ' + mName + ' closed');
        }
        
        this.sendUserMessage = function(lFrom, lContent, lTo)
        {
            var lMsgObj = new SMessage(SignalingMessageType.UserMessage, lContent, lFrom.getId());
            if(typeof lTo !== 'undefined' && lTo !== null && lTo !== -1)
            {
                mConnection[lTo].emit(MESSAGE_NAME, lMsgObj);
            }else{
                for(var id in mConnection)
                {
                    mConnection[id].emit(MESSAGE_NAME, lMsgObj);
                }
            }
        }
        
        function sendUserJoined(lConnection)
        {
            for(var id in mConnection)
            {
                var lMsgObj = new SMessage(SignalingMessageType.UserJoined, "", lConnection.getId());
                mConnection[id].emit(MESSAGE_NAME, lMsgObj);
            }
        }
        
        function sendUserLeft(lConnection)
        {
            for(var id in mConnection)
            {
                var lMsgObj = new SMessage(SignalingMessageType.UserLeft, "", lConnection.getId());
                mConnection[id].emit(MESSAGE_NAME, lMsgObj);
            }
        }
    }
    
    function Connection(lId)
    {
        var self = this;
        var mSocket = null;
        
        //socket.io connection etablished but not yet joined or opened a room
        var mConnecting = true;
        var mRoomOwner = false;
        var mOwnId = lId;
        
        this.getId = function()
        {
            return mOwnId;
        }
        
        //reference to the joined room
        var mRoom = null;
        
        //Called during socket.io connection event
        this.init = function(lSocket)
        {
            mSocket = lSocket;

            mSocket.on(MESSAGE_NAME, function(lMsg) {
                
                console.log('REC: ' + JSON.stringify(lMsg));
                if(lMsg.type == SignalingMessageType.OpenRoom)
                {
                    onOpenRoom(lMsg.content);
                }else if(lMsg.type == SignalingMessageType.JoinRoom)
                {
                    onJoinRoom(lMsg.content);
                }else if(lMsg.type == SignalingMessageType.UserMessage)
                {
                    onUserMessage(lMsg.content, lMsg.id);
                }else
                {
                    console.log("Unknown message: " + JSON.stringify(lMsg));
                }
            });
            
            //disconnect evet
            mSocket.on('disconnect', function() {
                onDisconnect();
            });
        };
        
        /**
         * Called if the connection is in a room that is closed.
         */
        this.disconnect = function()
        {
            mSocket.disconnect();
        };
        
        this.emit = function(lType, lMsg)
        {
            mSocket.emit(lType, lMsg);
        };
        
        function onOpenRoom(lRoomName)
        {
            console.log('Try to openn room ' + lRoomName);
            mConnecting = false;
            
            //room name still free? 
            if((lRoomName in gRooms) == false)
            {
                mRoomOwner = true;
                
                //open the room
                mRoom = new Room(lRoomName);
                mRoom.addConnection(self);
                
                
                console.log('room opened: ' + lRoomName);
                
                var lMsgObj = new SMessage(SignalingMessageType.Connected, mRoom.getName(), mOwnId);
                mSocket.emit(MESSAGE_NAME, lMsgObj);
                console.log('SND: ' + JSON.stringify(lMsgObj));
            }
            else
            {
                //disconnect the user if it failed
                mSocket.disconnect();
            }
        }
        
        function onJoinRoom(lRoomName)
        {
            mConnecting = false;
            //does the room exist?
            if(lRoomName in gRooms)
            {
                mRoomOwner = false;
                mRoom = gRooms[lRoomName];
                //join the room
                mRoom.addConnection(self);
                
                console.log('user ' + mOwnId + ' joined room: ' + lRoomName);
                
                var lMsgObj = new SMessage(SignalingMessageType.Connected, mRoom.getName(), mOwnId);
                mSocket.emit(MESSAGE_NAME, lMsgObj);
                console.log('SND: ' + JSON.stringify(lMsgObj));
            }
            else
            {
                
                console.log('user ' + mOwnId + ' will be disconnected. Room ' + lRoomName + " unknown");
                //disconnect the user if it failed
                mSocket.disconnect();
            }
        }
        
        function onUserMessage(lContent, lTo)
        {
            if(mRoom != null) //check if user is actually in a room
            {
                mRoom.sendUserMessage(self, lContent, lTo);
            }
        }
        
        function onDisconnect()
        { 
            if(mRoom != null)
            {
                if(mRoomOwner){
                    
                    console.log('Room owner ' + mOwnId + ' disconnecting. Closing room ');
                    mRoom.remConnection(self);
                    mRoom.close();
                }else{
                    console.log('user ' + mOwnId + ' disconnecting');
                    mRoom.remConnection(self);
                }
            }

            console.log('user ' + mOwnId + ' disconnected');
        }
        
        
        
    }
}

