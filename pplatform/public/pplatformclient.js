//pure platform interface
//contains still some jquery commands accessing the GUI they will move into the index.html in the future
    
var TAG = {};
TAG.CONTROLLER_REGISTER = "PLATFORM_CONTROLLER_REGISTER";
TAG.CONTROLLER_DISCOVERY = "PLATFORM_CONTROLLER_DISCOVERY";

function ControllerDiscoveryMessage(lConnectionId, lUserId, lName)
{
    this.connectionId = lConnectionId;
    this.userId = lUserId;
    this.name = lName;
}

TAG.CONTROLLER_LEFT = "PLATFORM_CONTROLLER_LEFT";
TAG.VIEW_DISCOVERY = "PLATFORM_VIEW_DISCOVERY";
TAG.ENTER_GAME = "PLATFORM_ENTER_GAME";
TAG.SERVER_FULL = "PLATFORM_SERVER_FULL";
TAG.NAME_IN_USE = "PLATFORM_NAME_IN_USE";

//special message to inform the games and other things connected to the platform
//that the connection was disconnected. current ui will reload the page
//in the future this could be used to show errors
TAG.DISCONNECTED = "PLATFORM_DISCONNECTED";


 function Controller(lConnectionId, lUserId, lName)
 {
     var self = this;
     
     var connectionId = lConnectionId;
     var userId = lUserId;
     var name = lName;
     
     this.getName = function()
     {
         return name;
     };     
     
     this.getId = function()
     {
         return self.userId;
     };
     
     
     this.toString = function()
     {
         return "Controller[connectionId:" + connectionId+ ", userId:" + userId+ ", "+ ", name:" + name;
     };
 }
 
 
 function PPlatform()
 {
     
    var self = this;
    var mIsConnected = false;
    this.isConnected = function()
    {
        return mIsConnected;
    };
    
    
    this.VIEW_USER_ID = 0;
    this.HOST_CONTROLLER_USER_ID = 1;
    
    //only available on the view side for now
    var mControllers = {};
    
    /**Returns an object containing connection id as key and controller
     * information as values.
     * 
     * @returns {c|PPlatform.mControllers}
     */
    this.getControllers = function()
    {
        return mControllers;
    };
    
    //This value will be null until a view was discovered. Only then a controller is fully initialized
    var mViewId = null;
    
    
    var mActiveGame = null;
    
    var serverUrl = "http://because-why-not.com:3001";
    if(window.location.href.indexOf("localhost") > -1) {
        serverUrl = "http://localhost:3001";
    }
    
    var mMessageListener = [];
    var sigChan = new Netgroup(serverUrl);
    this.getOwnUserId = function()
    {
        return mOwnUserId;
    };
    this.getOwnId = function() //keeps old code from breaking
    {
        return mOwnUserId;
    };
    this.getOwnConnectionId = function()
    {
        return sigChan.getOwnId();
    };
    
    var mOwnUserId = -1;
    //the name the user chose. 
    var mOwnName = null;
    
    var mIsHostController = false;
    
    function onClose()
    {
        handleMessage(TAG.DISCONNECTED, null, -1);
        //disconnected. clean up all data
        mControllers = {};
        
        
    }
        
    this.isHostController = function()
    {
        return mIsHostController;
    };
    
    this.enterGame = function(lGame)
    {
        mActiveGame = lGame;
        this.sendMessage(TAG.ENTER_GAME, lGame);
    };
    
    this.join = function(key, name)
    {
        mOwnName = name;
        sigChan.connect(key, OnNetgroupMessageInternal);
        mIsHostController = true;
    };
    
    this.disconnect = function()
    {
        sigChan.close();
    };
    this.sendMessageObj = function(lTag, lObj, lTo)
    {
        self.sendMessage(lTag, JSON.stringify(lObj), lTo);
    };
    this.sendMessage = function(lTag, lContent, lTo)
    {
        //the content is in typesafe platforms defined as "string"
        //it should always be a string even if it is empty
        if(typeof lContent === "undefined" || lContent == null)
        {
            lContent = "";
        }
        var msg = {};
        msg.tag = lTag;
        msg.content = lContent; 
        
        self.Log("Snd: TAG: " + lTag + " data: " + JSON.stringify(msg) + " to " + lTo);
        sigChan.sendMessageTo(JSON.stringify(msg), lTo);  
    };
    this.addMessageListener = function(lListener)
    {
        mMessageListener.push(lListener);
    };
    
    /** Will handle all incomming messages + send them to the listener outside of pplatform
     * 
     * All tasks that are unique to the netgroup commands don't belong here
     */
    function handleMessage(lTag, lContent, lId)
    {
        //events to handle before the content gets it
        
        if(lTag == TAG.ENTER_GAME){
            ShowGame(lContent);
        }else if(lTag == TAG.SERVER_FULL)
        {
            self.disconnect();
        }else if(lTag == TAG.NAME_IN_USE)
        {
            self.disconnect();
        }else if(lTag == TAG.CONTROLLER_DISCOVERY)
        {
            var controllerDiscoveryData = JSON.parse(lContent);
            var c = new Controller(controllerDiscoveryData.connectionId, controllerDiscoveryData.userId, controllerDiscoveryData.name);
            mControllers[controllerDiscoveryData.userId] = c;
            if(self.getOwnConnectionId() == controllerDiscoveryData.connectionId)
            {
                mOwnName = controllerDiscoveryData.name;
                mOwnUserId = controllerDiscoveryData.userId;
            }
        }
        else if(lTag == TAG.VIEW_DISCOVERY)
        {
            mViewId = lId; //store the id for later
            
            
            var name = mOwnName;
            if(name === null || name === "" || typeof name === "undefined")
            {
                name = ""; //empty name will be replaced by the server
            }
            //register as controller at the view
            var controllerRegisterData = {"name": name};
            self.sendMessage(TAG.CONTROLLER_REGISTER, JSON.stringify(controllerRegisterData), mViewId);
            
        }else if(lTag == TAG.CONTROLLER_LEFT){
            delete mControllers[lId];
        }
        
        //send the event out to the game and ui
        for(var i = 0; i < mMessageListener.length; i++)
        {
            mMessageListener[i](lTag, lContent, lId);
        }
        
    }
    
    function ShowGame(lGameName)
    {
        mActiveGame = lGameName;
        var lGameNameUrl = "./game/" + lGameName;
        $('#contentframe').attr("src",  lGameNameUrl);
            
        self.Log("opening " +  lGameNameUrl);
    }
    
    /**
     * this is the message handler based on Netgroup. Only the content ofer user messages will be send to the games outside of platform via mMessageListener
     */
    function OnNetgroupMessageInternal(lType, lId, lMsg)
    {
        if(lType == SignalingMessageType.Connected)
        {
            self.Log("Connected");
            mIsConnected = true;
            $('#openroom').attr("hidden", true);
            $('#joinspan').attr("hidden", true);
            $('#connectedspan').attr("hidden", false);
            //ShowGame("gamelist");
            
        }else if(lType == SignalingMessageType.UserMessage)
        {
            var msgObj = JSON.parse(lMsg);
            
            self.Log("Rec: TAG: " + msgObj.tag + " data:" + JSON.stringify(msgObj.content) + " from " + lId);
            handleMessage(msgObj.tag, msgObj.content, lId);
            
            
        }else if(lType == SignalingMessageType.Closed)
        {
            $('#openroom').attr("hidden", false);
            $('#joinspan').attr("hidden", false);
            $('#connectedspan').attr("hidden", true);
            mIsConnected = false;
            onClose();
            self.Log("Disconnected");
        }else if(lType == SignalingMessageType.UserJoined)
        {
            self.Log("User " + lId + " joined");
            
            
            //controller ignore these so far
        }else if(lType == SignalingMessageType.UserLeft)
        {
            self.Log("User " + lId + " left");
            
            //controller ignore these so far
        }else{
            self.Log("Invalid message received. type: " + lType + " content: " + lMsg);
        }
    }
    function GetRandomKey()
    {
        
        var result = "";
        for(var i = 0; i < 6; i++)
        {
            result += String.fromCharCode(65 + Math.round(Math.random() * 25));
        }
        return result;
    }
    
    this.Log = function(msg)
    {
        console.debug(msg);
    };
 }