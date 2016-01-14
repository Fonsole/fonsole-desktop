//pure platform interface
//contains still some jquery commands accessing the GUI they will move into the index.html in the future
    
 function PPlatform()
 {
    var gConnected = false;
    
    var serverUrl = "http://because-why-not.com:3001";
    if(window.location.href.indexOf("localhost") > -1) {
        serverUrl = "http://localhost:3001";
    }
    
    var mMessageListener = [];
    var sigChan = new Netgroup(serverUrl);
    
    
    
    
    this.startAsView = function()
    {
        
        var key = GetRandomKey();
        
        Log("Opening room " + key + " ...");
        sigChan.open(key, OnSignalingMessageInternal);
        $('#gameid').val(key);
        
    };
    
    this.startAsController = function(key)
    {
        sigChan.connect(key, OnSignalingMessageInternal);
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
        //TODO: Handle tags
        sigChan.sendMessage(JSON.stringify(msg));  
    };
    this.addMessageListener = function(lListener)
    {
        mMessageListener.push(lListener);
    };
    
    
    function OnSignalingMessageInternal(lType, lMsg)
    {
        if(lType == SignalingMessageType.Connected)
        {
            Log("Connected");
            gConnected = true;
          
          
            $('#openroom').attr("hidden", true);
            $('#joinspan').attr("hidden", true);
            $('#connectedspan').attr("hidden", false);
        }else if(lType == SignalingMessageType.UserMessage)
        {
            Log("Message: " + lMsg);
            var msgObj = JSON.parse(lMsg);
            
            for(var i = 0; i < mMessageListener.length; i++)
            {
                mMessageListener[i](msgObj.tag, msgObj.content);
            }
            
        }else if(lType == SignalingMessageType.Closed)
        {
            $('#openroom').attr("hidden", false);
            $('#joinspan').attr("hidden", false);
            $('#connectedspan').attr("hidden", true);
            gConnected = false;
            Log("Disconnected");
        }else{
            Log("Invalid message received. type: " + lType + " content: " + lMsg);
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
    
 }