





module.exports = function()
{
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
        io.on('connection', function(lSocket) {
            
            connectionCount++;
            console.log('a user ' + connectionCount + ' connected');
            var connection = new Connection(connectionCount);
            connection.init(lSocket);
        });
    }
    
    
    function Room(lName)
    {
        var mName = lName;
        var mConnection = {};
        
        //add to the global room list
        gRooms[mName] = this; 
        
        this.addConnection = function(lConnection)
        {
            mConnection[lConnection.getId()] = lConnection;
            sendUserJoined(lConnection);
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
            var lMsgObj = {};
            lMsgObj.content = lContent;
            lMsgObj.id = lFrom.getId();
            if(typeof lTo !== 'undefined' && lTo !== null)
            {
                mConnection[id].emit('user message', JSON.stringify(lMsgObj));
            }else{
                var txt = JSON.stringify(lMsgObj);
                for(var id in mConnection)
                {
                    mConnection[id].emit('user message', txt);
                }
            }
        }
        
        function sendUserJoined(lConnection)
        {
            var msg = lConnection.getId();
            for(var id in mConnection)
            {
                mConnection[id].emit('user joined', msg);
            }
        }
        
        function sendUserLeft(lConnection)
        {
            var msg = lConnection.getId();
            for(var id in mConnection)
            {
                mConnection[id].emit('user left', msg);
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

            //event to open a new room and add the user
            //user will get disconnected immediately if there isn't a room available (part of the definition of SignalingChan)
            mSocket.on('open room', function(lMsg) {
                onOpenRoom(lMsg);
            });
            
            //join event. either joins an existing room. if there is none -> disconnect
            mSocket.on('join room', function(lMsg) {
                onJoinRoom(lMsg);
            });
            
            //msg sends a message to everyone in the room
            mSocket.on('user message', function(lMsg) {
                onUserMessage(JSON.parse(lMsg));
            });
            
            //disconnect evet
            mSocket.on('disconnect', function() {
                onDisconnect();
            });
        }
        
        /**
         * Called if the connection is in a room that is closed.
         */
        this.disconnect = function()
        {
            mSocket.disconnect();
        }
        
        this.emit = function(lType, lMsg)
        {
            mSocket.emit(lType, lMsg);
        }
        
        function onOpenRoom(lRoomName)
        {
            mConnecting = false;
            
            //room name still free? 
            if((lRoomName in gRooms) == false)
            {
                mRoomOwner = true;
                
                //open the room
                mRoom = new Room(lRoomName);
                mRoom.addConnection(self);
                
                
                console.log('room opened: ' + lRoomName);
                
                var lMsgObj = {};
                lMsgObj.name = mRoom.name;
                lMsgObj.id = mOwnId;
                mSocket.emit('room opened', JSON.stringify(lMsgObj));
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
                
                var lMsgObj = {};
                lMsgObj.name = mRoom.name;
                lMsgObj.id = mOwnId;
                mSocket.emit('room joined', JSON.stringify(lMsgObj));
            }
            else
            {
                //disconnect the user if it failed
                mSocket.disconnect();
            }
        }
        
        function onUserMessage(lMsgObj)
        {
            if(mRoom != null) //check if user is actually in a room
            {
                mRoom.sendUserMessage(self, lMsgObj.content, lMsgObj.id);
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




module.exports.test = "testmessages";