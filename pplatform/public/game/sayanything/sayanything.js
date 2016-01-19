




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
        
        //user id of the answer the judge has chosen
        this.judgedAnswerId = null;
        
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
            ShowAnswers : 3,
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
    
    //sent after the judge chooses a question
    SayAnything.Message.Question = function(lQuestion)
    {
        this.question = lQuestion;
    }
    SayAnything.Message.Question.TAG = "SayAnything_Question";
    
    //sent after the controllers enter an answer and press the confirm button
    SayAnything.Message.Answer = function(lAnswer)
    {
        this.answer = lAnswer;
    }
    SayAnything.Message.Answer.TAG = "SayAnything_Answer";
    
    
    SayAnything.Message.Judge = function(lPlayerId)
    {
        this.playerId = lPlayerId;
    }
    SayAnything.Message.Judge.TAG = "SayAnything_Judge";
    
    
    SayAnything.Message.Vote = function(lVotePlayerId1, lVotePlayerId2)
    {
        this.votePlayerId1 = lVotePlayerId1;
        this.votePlayerId2 = lVotePlayerId2;
    }
    SayAnything.Message.Vote.TAG = "SayAnything_Vote";
    
    
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
            }else if(lTag == SayAnything.Message.Question.TAG)
            {
                if(mData.state == SayAnything.GameState.Questioning)
                {
                    console.debug("question received: " + lContent.question);
                    mData.question = lContent.question;
                    mData.state = SayAnything.GameState.Answering;
                    refreshState();
                }else{
                    console.debug("Received a question during invalid state " + mData.state);
                }
            }else if(lTag == SayAnything.Message.Answer.TAG)
            {
                if(mData.state == SayAnything.GameState.Answering)
                {
                    console.debug("answer received: " + lContent.answer);
                    if(lFrom in mData.answers)
                    {
                        //already an asnwer received. ignored
                        console.debug("already an answer received. ignored " + lContent.answer);
                    }else{
                        mData.answers[lFrom] = lContent.answer;
                        
                        if(Object.keys(mData.answers).length >= Object.keys(gPlatform.getControllers()).length - 1)
                        {
                            //received same amount of answers as controllers available -> next state
                            
                            mData.state = SayAnything.GameState.ShowAnswers;
                            
                            setTimeout(function()
                            {
                                mData.state = SayAnything.GameState.Judging;
                                refreshState();
                            }, 10000);
                        }
                        
                        refreshState();
                    }
                }else{
                    console.debug("Received an answer during invalid state " + mData.state);
                }
            }else if(lTag == SayAnything.Message.Judge.TAG)
            {
                if(mData.state == SayAnything.GameState.Judging)
                {
                    console.debug("question received: " + lContent.question);
                    mData.judgedAnswerId = lContent.playerId;
                    mData.state = SayAnything.GameState.Voting;
                    refreshState();
                }else{
                    console.debug("Received a juding response during invalid state " + mData.state);
                }
            }
            else if(lTag == SayAnything.Message.Vote.TAG)
            {
                if(mData.state == SayAnything.GameState.Voting)
                {
                    console.debug("vote from " + lFrom + " received: " + lContent.votePlayerId1 + " and " + lContent.votePlayerId2);
                    if(lFrom in mData.votes)
                    {
                        //already an answer received. ignored
                        console.debug("already an votes received. ignored " + lContent.answer);
                    }else{
                        mData.votes[lFrom] = [lContent.votePlayerId1, lContent.votePlayerId1];
                        
                        if(Object.keys(mData.answers).length >= Object.keys(gPlatform.getControllers()).length - 1)
                        {
                            //received same amount of votes as controllers available -> next state
                            CalculateScore();
                            mData.state = SayAnything.GameState.ShowWinner;
                            setTimeout(function()
                            {
                                mData.state = SayAnything.GameState.ShowScore;
                                refreshState();
                            }, 10000);
                        }
                        
                        refreshState();
                    }
                }else{
                    console.debug("Received an answer during invalid state " + mData.state);
                }
            }
        }
        
        function CalculateScore()
        {
            
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
        }else if(lSharedData.state == SayAnything.GameState.Answering)
        {
            console.log("show Answering");
            $('#Answering').attr("hidden", false);
        }else if(lSharedData.state == SayAnything.GameState.ShowAnswers)
        {
            console.log("show ShowAnswers");
            $('#ShowAnswers').attr("hidden", false);
            answerListFill(lSharedData.answers);
            
        }else if(lSharedData.state == SayAnything.GameState.Judging)
        {
            console.log("show Judging");
            $('#Judging').attr("hidden", false);
            answerListFill(lSharedData.answers);
            
        }else if(lSharedData.state == SayAnything.GameState.Voting)
        {
            console.log("show Voting");
            $('#Voting').attr("hidden", false);
            answerListFill(lSharedData.answers);
            
        }else if(lSharedData.state == SayAnything.GameState.ShowWinner)
        {
            console.log("show ShowWinner");
            $('#ShowWinner').attr("hidden", false);
            answerListFill(lSharedData.answers);
            
        }else if(lSharedData.state == SayAnything.GameState.ShowScore)
        {
            console.log("show ShowScore");
            $('#ShowScore').attr("hidden", false);
            answerListFill(lSharedData.answers);
            
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
        
        //if question is set -> fill in the question parts in the ui
        if(lSharedData.question != null)
            $('.chosenQuestion').empty().append(lSharedData.question);
        
        
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