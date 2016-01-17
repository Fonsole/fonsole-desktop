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
    
    var mKnownConnections = null;
    
    var TAG_CONTROLLER_DISCOVERY = "PLATFORM_CONTROLLER_DISCOVERY";
    var TAG_CONTROLLER_LEFT = "PLATFORM_CONTROLLER_LEFT";
    var TAG_ENTER_GAME = "PLATFORM_ENTER_GAME";
    var TAG_EXIT_GAME = "PLATFORM_EXIT_GAME";
    
    var mActiveGame = null;
    
    var serverUrl = "http://because-why-not.com:3001";
    if(window.location.href.indexOf("localhost") > -1) {
        serverUrl = "http://localhost:3001";
    }
    
    var mMessageListener = [OnUserMessage];
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
        sigChan.open(key, OnSignalingMessageInternal);
        $('#gameid').val(key);
        mIsView = true;
        mIsHostController = false;
    };
    
    this.startAsController = function(key)
    {
        sigChan.connect(key, OnSignalingMessageInternal);
        mIsView = false;
        mIsHostController = true;
    };
    
    this.disconnect = function()
    {
        sigChan.close();
    };
    
    this.sendMessage = function(lTag, lContent)
    {
        var msg = {};
        msg.tag = lTag;
        msg.content = lContent; 
        
        sigChan.sendMessage(JSON.stringify(msg));  
    };
    this.addMessageListener = function(lListener)
    {
        mMessageListener.push(lListener);
    };
    
    function OnUserMessage(lTag, lContent)
    {
        if(lTag == TAG_ENTER_GAME)
        {
            ShowGame(lContent);
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
    function OnSignalingMessageInternal(lType, lId, lMsg)
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
            self.Log("Message: " + lMsg);
            var msgObj = JSON.parse(lMsg);
            
            for(var i = 0; i < mMessageListener.length; i++)
            {
                mMessageListener[i](msgObj.tag, msgObj.content, lId);
            }
            
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
        }else if(lType == SignalingMessageType.UserLeft)
        {
            self.Log("User " + lId + " disconnected");
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