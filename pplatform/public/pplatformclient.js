//pure platform interface
//contains still some jquery commands accessing the GUI they will move into the index.html in the future
    
var TAG = {};
TAG.CONTROLLER_REGISTER = "PLATFORM_CONTROLLER_REGISTER";
TAG.CONTROLLER_DISCOVERY = "PLATFORM_CONTROLLER_DISCOVERY";
TAG.CONTROLLER_LEFT = "PLATFORM_CONTROLLER_LEFT";
TAG.VIEW_DISCOVERY = "PLATFORM_VIEW_DISCOVERY";
TAG.ENTER_GAME = "PLATFORM_ENTER_GAME";
TAG.EXIT_GAME = "PLATFORM_EXIT_GAME";


 function Controller(lId, lName)
 {
     var self = this;
     //TODO: this should be private
     this.id = lId;
     var name = lName;
     
     this.getName = function()
     {
         return name;
     };     
     
     this.getId = function()
     {
         return self.id;
     };
 }
 
 
 function PPlatform()
 {
     
    var self = this;
    var gConnected = false;
    
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
    this.getOwnId = function()
    {
        return sigChan.getOwnId();
    };
    
    //the name the user chose. 
    var mOwnName = null;
    
    var mIsView = false;
    var mIsHostController = false;
    
    this.isView = function()
    {
        return mIsView;
    }
    this.isController = function()
    {
        return mIsView == false;
    }
    this.isHostController = function()
    {
        return mIsHostController;
    }
    
    this.enterGame = function(lGame)
    {
        mActiveGame = lGame;
        this.sendMessage(TAG.ENTER_GAME, lGame);
    };
    
    this.startAsView = function()
    {
        
        var key = GetRandomKey();
        
        self.Log("Opening room " + key + " ...");
        sigChan.open(key, OnNetgroupMessageInternal);
        $('#gameid').val(key);
        mIsView = true;
        mIsHostController = false;
    };
    
    this.startAsController = function(key, name)
    {
        mOwnName = name;
        sigChan.connect(key, OnNetgroupMessageInternal);
        mIsView = false;
        mIsHostController = true;
    };
    
    this.disconnect = function()
    {
        sigChan.close();
    };
    
    this.sendMessage = function(lTag, lContent, lTo)
    {
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
        }else if(lTag == TAG.EXIT_GAME){
            //todo handle exit game command -> switch view back to game list?
        }else if(lTag == TAG.CONTROLLER_DISCOVERY)
        {
            var controllerDiscoveryData = JSON.parse(lContent);
            var c = new Controller(controllerDiscoveryData.id, controllerDiscoveryData.name);
            mControllers[controllerDiscoveryData.id] = c;
        }else if(lTag == TAG.VIEW_DISCOVERY)
        {
            mViewId = lId; //store the id for later
            
            
            var name = mOwnName;
            if(name === null || name === "" || typeof name === "undefined")
            {
                name = "player " + self.getOwnId();
            }
            //register as controller at the view
            var controllerRegisterData = {"name": name};
            self.sendMessage(TAG.CONTROLLER_REGISTER, JSON.stringify(controllerRegisterData), mViewId);
            
        }else if(lTag == TAG.CONTROLLER_REGISTER)
        {
            //should only received by the view only.
            //this message is sent by a new controller that joined already
            //and now wants to be registerd as controller
            
            //TODO: get the controller info and send out
            //a controller discovery to everyone
            //later we could check username / facebook and so on here
            
            //this could also be moved to the server
            
            var lControllerRegisterData = JSON.parse(lContent);
            
            
            self.Log("New controller registered: " + lControllerRegisterData.name);
            //so far the controller registers only via a name
            //the view will add the controllers id so everyone knows how to
            //address this controller
            var controllerDiscoveryData = {"id": lId, "name": lControllerRegisterData.name};
            
            //broadcast to everyone. this instance will receive itself the message and then add the controller to the list
            self.sendMessage(TAG.CONTROLLER_DISCOVERY, JSON.stringify(controllerDiscoveryData));
            
            //send a discovery message to the new controller about the existing ones
            for(var contrId in mControllers)
            {
                var controller = mControllers[contrId];
                var controllerDD = {"id": controller.getId(), "name": controller.getName()}; 
                self.sendMessage(TAG.CONTROLLER_DISCOVERY, JSON.stringify(controllerDD), lId);
            }
            
            //last step: inform the controller about the currently active game
            self.sendMessage(TAG.ENTER_GAME, mActiveGame, lId);
            
        }else if(lTag == TAG.CONTROLLER_LEFT){
            delete mControllers[lId];
        }
        
        //send the event out to the game and ui
        for(var i = 0; i < mMessageListener.length; i++)
        {
            mMessageListener[i](lTag, lContent, lId);
        }
        
    }
    
    function ShowGame(lGameNameUrl)
    {
        mActiveGame = lGameNameUrl;
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
            gConnected = true;
            $('#openroom').attr("hidden", true);
            $('#joinspan').attr("hidden", true);
            $('#connectedspan').attr("hidden", false);
            ShowGame("./gamelist.html");
            
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
            gConnected = false;
            self.Log("Disconnected");
        }else if(lType == SignalingMessageType.UserJoined)
        {
            self.Log("User " + lId + " joined");
            
            
            if(mIsView)
            {
                //send out view discovery event
                self.sendMessage(TAG.VIEW_DISCOVERY, "", lId);
            }
            //controller ignore these so far
        }else if(lType == SignalingMessageType.UserLeft)
        {
            self.Log("User " + lId + " left");
            
            if(mIsView)
            {
                handleMessage(TAG.CONTROLLER_LEFT, null, lId);
            }
            
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
        $('#messages').append($('<li>').text(msg));
        console.debug(msg);
    }
 }