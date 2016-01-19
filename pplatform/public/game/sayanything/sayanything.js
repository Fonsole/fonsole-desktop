




//Game data
    var SayAnything = {};
    SayAnything.Data = {}; //namespace for different kind of data: Shared and Local
    SayAnything.Message = {};


    /**Will be used by all controllers and view. It can be only changed by the view and if it changes it will
     * send the new version to all connected controllers. (if needed)
     * 
     */
    SayAnything.Data.Shared = function()
    {
        //current state
        this.state = SayAnything.GameState.WaitForStart;
        
        //user id of the judge. only the connection id for now
        this.judgeUserId = null;
        
        //simply the text of the question
        this.question = null;
        
        //will contain the player id as key and then the answer.
        this.answers = {};
        
        //key: user id the votes are from + value -> a list of user id's the votes are for in the answer list
        this.votes = {};
        
        //will be a list with the players score  (independend if the player is sitll around or not)
        this.score = {};
    }

    /** The states the game can be in.
     * 
     */
     SayAnything.GameState = {
            WaitForStart : 0,
            Questioning : 1,
            Answering : 2,
            ShowAnswer : 3,
            Judging : 4,
            Voting : 5,
            ShowWinner : 6,
            ShowScore : 7
        }
    
//pre build messages content so it is clear what a message is suppose to contain
   
   //updates view and client after the shared data changed
    SayAnything.Message.SharedDataUpdate = function(lSharedData)
    {
        this.sharedData = lSharedData;
    }
    SayAnything.Message.SharedDataUpdate.TAG = "SayAnything_SharedDataUpdate";
    
    //message to the view that a controller clicked the start game button
    SayAnything.Message.StartGame = function()
    {
        //no content
    }
    SayAnything.Message.StartGame.TAG = "SayAnything_StartGame";
    
    //notifying the view that a controller finished loading the game and is ready to process messages
    SayAnything.Message.GameLoaded = function()
    {
        //no content
    }
    SayAnything.Message.GameLoaded.TAG = "SayAnything_GameLoaded";
    
    SayAnything.Message.Question = function(lQuestion)
    {
        this.question = lQuestion;
    }
    SayAnything.Message.Question.TAG = "SayAnything_Question";
    
    
    
    
    //var startGameMessage = new SayAnything.Message.StartGame();
    //alert(JSON.stringify(startGameMessage));
    
    
    
//Game logic
    /**Will construct the Say Anything game logic
     * 
     */
    SayAnything.Logic = function()
    {
        var mData = new SayAnything.Data.Shared();
        this.getSharedData = function(){return mData;}
        
        
        this.onMessage = function(lTag, lContent, lFrom)
        {
            //game messages for the view 
            if(lTag == SayAnything.Message.StartGame.TAG)
            {
                console.log("start game");
                
                //TODO: dice
                mData.judgeUserId = lFrom;
                mData.state = SayAnything.GameState.Questioning;
                refreshState();
            }else if(lTag == TAG.CONTROLLER_DISCOVERY)
            {
                refreshState();
            }else if(lTag == TAG.CONTROLLER_LEFT)
            {
                refreshState();
            }else if(lTag == SayAnything.Message.GameLoaded.TAG)
            {
                refreshState();
            }
        }
        
        
        function refreshState()
        {
            gPlatform.sendMessage(SayAnything.Message.SharedDataUpdate.TAG , new SayAnything.Message.SharedDataUpdate(mData));
        }
        
        
        refreshState();
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
//UI SYNC -> methods to update the ui based on changes in the game data
    function refreshPlayerList()
    {
        $('#playerlist').empty();
        
        var controllers = gPlatform.getControllers();
        for(var id in controllers )
        {
            $('#playerlist').append( "<li>" + controllers[id].name + "</li>" );
        }
    }
    function refreshUi(lSharedData, lLocalData)
    {
        //TODO: UI to data connection
        console.debug("UI refresh");
        
        //hide everything -> show later only what is needed
        $('.view').attr("hidden", true);
        $('.hostcontroller').attr("hidden", true);
        $('.controller').attr("hidden", true);
        $('.judgecontroller').attr("hidden", true);
            

        //hide all states. TODO: remove that later to avoid flickering during state updates
        $('.stateview').attr("hidden", true);
        
        //show the correct state view
        if(lSharedData.state == SayAnything.GameState.WaitForStart)
        {
            console.log("show WaitForStart");
            $('#WaitForStart').attr("hidden", false);
        }else if(lSharedData.state == SayAnything.GameState.Questioning)
        {
            console.log("show Questioning");
            $('#Questioning').attr("hidden", false);
        }else{
            console.debug("ERROR: GUI doesn't know state " + lSharedData.state);
        }
        if(gPlatform.isView())
        {
            $('.view').attr("hidden", false);
        }else if(lSharedData.judgeUserId != null && lSharedData.judgeUserId == gPlatform.getOwnId())
        {
            $('.judgecontroller').attr("hidden", false);
            $('.hostcontroller').attr("hidden", false);
        }else{
            $('.controller').attr("hidden", false);
            $('.hostcontroller').attr("hidden", false);
        }
        
        
        //if there is a judge -> add the judge player to everything with class name "judgeName"
        if(lSharedData.judgeUserId != null)
        {
            $('.judgeName').empty();
            $('.judgeName').append("Player " + lSharedData.judgeUserId);
        }
        
    }
 
    
    

    var gPlatform = null;
    function InitPlatform()
    {
        gPlatform = parent.gPlatform;
        gPlatform.addMessageListener(onMessage);
        console.log("Game listener added");
        if(gPlatform.isView() == false)
        {
            gPlatform.sendMessage(SayAnything.Message.GameLoaded.TAG, new SayAnything.Message.GameLoaded());
        }
    }
    
    function onMessage(lTag, lContent, lFrom)
    {
        if(gPlatform.isView())
        {
            //platform specific messages
            if(lTag == TAG.CONTROLLER_DISCOVERY || lTag == TAG.CONTROLLER_LEFT)
            {
                refreshPlayerList();
            }
            
            gLogic.onMessage(lTag, lContent, lFrom);
        }
        
        
        //data updated -> refresh ui
        if(lTag == SayAnything.Message.SharedDataUpdate.TAG)
        {
            refreshUi(lContent.sharedData);
        }

    }
    
    var gLogic = null;
//called once after the page is loaded see index.html
function InitGame()
{
    //Setup of the platform and UI
    if(typeof parent.gPlatform !== 'undefined')
    {
        InitPlatform();
        //first ui refresh with empty data -> hides everything visible for artists only
        refreshPlayerList();
        
        if(gPlatform.isView())
        {
            gLogic = new SayAnything.Logic();
            refreshUi(gLogic.getSharedData());
        }
        
    }
}