




module.exports = function()
{
    var Server = require('socket.io');
    var io;
    
        
    //just for debugging purposes
    var connectionCount = 0;

    //here we keep track of the rooms
    var rooms = {};
    
    
    
    this.listen = function(lHttpOrPort)
    {
        io = new Server(lHttpOrPort);
        
        
        //wait for connections via socket.io. if a connection is etablished setup all the handlers for messages
        io.on('connection', function(socket) {
            
            connectionCount++;
            console.log('a user ' + connectionCount + ' connected');
            
            
            var lRoomOwner = false;
            var lOwnId = -1;
            var lRoom = null;

            //event to open a new room and add the user
            //user will get disconnected immediately if there isn't a room available (part of the definition of SignalingChan)
            socket.on('open room', function(msg) {
                //room name still free? 
                if((msg in rooms) == false)
                {
                    lRoomOwner = true;
                    lOwnId = connectionCount;
                    //open the room
                    lRoom = {};
                    lRoom.connection = {};
                    lRoom.connection[connectionCount] = socket;
                    lRoom.name = msg;
                    rooms[lRoom.name] = lRoom; //room owner is always index 0
                    
                    console.log('room opened: ' + msg);
                    
                    var answer = {};
                    answer.name = lRoom.name;
                    answer.id = lOwnId;
                    socket.emit('room opened', JSON.stringify(answer));
                }
                else
                {
                    //disconnect the user if it failed
                    socket.disconnect();
                }
            });
            
            //join event. either joins an existing room. if there is none -> disconnect
            socket.on('join room', function(msg) {
                //does the room exist?
                if(msg in rooms)
                {
                    lOwnId = connectionCount;
                    lRoom = rooms[msg];
                    //join the room
                    lRoom.connection[lOwnId] = socket;
                    lRoomOwner = false;
                    
                    console.log('user ' + lOwnId + 'joined room: ' + msg);
                    
                    var answer = {};
                    answer.name = lRoom.name;
                    answer.id = lOwnId;
                    socket.emit('room joined', JSON.stringify(answer));
                }
                else
                {
                    //disconnect the user if it failed
                    socket.disconnect();
                }
            });
            
            //msg sends a message to everyone in the room
            socket.on('umsg', function(msg) {
                
                if(lRoom != null) //check if user is actually in a room
                {
                    
                    for(var id in lRoom.connection)
                    {
                        lRoom.connection[id].emit('umsg', msg);
                    }
                }
            });
            
            //disconnect evet
            socket.on('disconnect', function() {
                
                if(lRoomOwner)
                {
                    console.log('Room owner ' + lOwnId + ' disconnecting. Cosing room ' + lRoom.name);
                    delete rooms[lRoom.name];
                    
                    for(var id in lRoom.connection)
                    {
                        if(id != lOwnId)
                        {
                            
                            console.log('room ' + lRoom.name + ' closing. Disconnect user ' + id);
                            lRoom.connection[id].disconnect();
                            console.log('room ' + lRoom.name + ' closing. Disconnected user ' + id);
                        }
                    }
                    
                    console.log('Room ' + lRoom.name + ' closed');
                }else{
                console.log('user ' + lOwnId + ' disconnecting');
                    
                    //TODO: send broadcast that a user left
                    //room still exists or was closed?
                    if(lRoom != null && lRoom.name in rooms)
                    {
                        //remove the element
                        delete lRoom.connection[lOwnId];
                    }
                }
                console.log('user ' + lOwnId + ' disconnected');
                
            });
        });
    }
}




module.exports.test = "testmessages";