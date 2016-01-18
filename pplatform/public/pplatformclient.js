//pure platform interface
//contains still some jquery commands accessing the GUI they will move into the index.html in the future
    
    
 function Controller()
 {
     this.id = -1;
     this.name = "player -1";
 }
 function PPlatform()
 {
     
    var self = this;
    var gConnected = false;
    
    //only available on the view side for now
    var mControllers = {};
    
    //This value will be null until a view was discovered. Only then a controller is fully initialized
    var mViewId = null;
    
    var TAG_CONTROLLER_DISCOVERY = "PLATFORM_CONTROLLER_DISCOVERY";
    var TAG_CONTROLLER_LEFT = "PLATFORM_CONTROLLER_LEFT";
    
    
    //will be send out by the view to inform a new controller how to send messages to the view
    var TAG_VIEW_DISCOVERY = "PLATFORM_VIEW_DISCOVERY";
    
    
    var TAG_ENTER_GAME = "PLATFORM_ENTER_GAME";
    var TAG_EXIT_GAME = "PLATFORM_EXIT_GAME";
    
    var mActiveGame = null;
    
    var serverUrl = "http://because-why-not.com:3001";
    if(window.location.href.indexOf("localhost") > -1) {
        serverUrl = "http://localhost:3001";
    }
    
    var mMessageListener = [];
    var sigChan = new Netgroup(serverUrl);
    
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
        this.sendMessage(TAG_ENTER_GAME, lGame);
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
    
    this.startAsController = function(key)
    {
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
        
        self.Log("Snd: " + JSON.stringify(msg) + " to " + lTo);
        sigChan.sendMessageTo(JSON.stringify(msg), lTo);  
    };
    this.addMessageListener = function(lListener)
    {
        mMessageListener.push(lListener);
    };
    
    /** Will handle all incomming messages + send them to the listener outside of pplatform
     * 
     * 
     */
    function handleMessage(lTag, lContent, lId)
    {
        //events to handle before the content gets it
        
        if(lTag == TAG_ENTER_GAME){
            ShowGame(lContent);
        }else if(lTag == TAG_EXIT_GAME){
            //todo handle exit game command -> switch view back to game list?
        }else if(lTag == TAG_CONTROLLER_DISCOVERY)
        {
            if(mIsView) {
                //send out view discovery event
                self.sendMessage(TAG_VIEW_DISCOVERY, "", lId);
                
                if(mActiveGame != null)
                {
                    self.sendMessage(TAG_ENTER_GAME, mActiveGame, lId);
                }
                var c = new Controller(lId, "player " + lId);
                mControllers[lId] = c;
            }
        }else if(lTag == TAG_VIEW_DISCOVERY)
        {
            mViewId = lId; //store the id for later
        }
        
        
        //send the event out to the game and ui
        for(var i = 0; i < mMessageListener.length; i++)
        {
            mMessageListener[i](lTag, lContent, lId);
        }
        
        //events to handle after the content gets it (e.g. if something gets removed. the game might want to still check 
        //the details of what gets removed before all traces are gone)
        if(lTag == TAG_CONTROLLER_LEFT){
            delete mControllers[lId];
        }
    }
    
    function ShowGame(lGameNameUrl)
    {
        
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
            self.Log("Rec: " + lMsg + " from " + lId);
            var msgObj = JSON.parse(lMsg);
            
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
            
            //if this is a view every discovered user must be a controller -> automatically inform that there is
            //a controller. the controllers itself don't need to send this for now (later they need to and add information
            //abouut color and name)
            if(mIsView)
            {
                handleMessage(TAG_CONTROLLER_DISCOVERY, null, lId);
                
            }
            //controller ignore these so far
        }else if(lType == SignalingMessageType.UserLeft)
        {
            self.Log("User " + lId + " left");
            
            if(mIsView)
            {
                handleMessage(TAG_CONTROLLER_LEFT, null, lId);
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