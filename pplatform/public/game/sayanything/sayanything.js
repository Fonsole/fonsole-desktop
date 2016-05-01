
    //global variables
//    var gLogic = null;
    var gPlatform = null;


    //called once after the page is loaded see index.html
    function initGame()
    {
        //Setup of the platform and UI
        if(typeof parent.gPlatform !== 'undefined')
        {
            gPlatform = parent.gPlatform;

            //send out a broadcast telling the say anything view + other controllers
            //that this controller loaded the game and can now receive game specific messages
            //the view will respond to this by sendingthe current game data
            gPlatform.sendMessageObj(SayAnything.Message.GameLoaded.TAG, new SayAnything.Message.GameLoaded());
        }
        //fills the ui with test data
        initTestUi();

        //all the ui needs is the possibility to receive messages
        //no direct access to the game logic is needed
        gPlatform.addMessageListener(onMessageUi);


        setInterval(function()
        {
            //refresh timer every sec
            if(gLastSharedData != null)
            {
                gLastSharedData.timeLeft = gLastSharedData.timeLeft - 1;
                $('.timeLeft').empty().append(gLastSharedData.timeLeft);
            }
        }, 1000);
    }





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
        var self = this;
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

        //key: user id (equals the id the answer of this user has in the "answers" object)
        //value: a list of user ids the vote came from (needed to show the color badges in the end)
        //(move to local data, view only?)
        this.votes = {};

        //scores in this round (move to local data, view only?)
        this.roundScore = {};

        //scores overall (move to local data, view only?)
        this.totalScore = {};

        this.timeLeft = 30;

         //functions to easily fill and read the data (ideall this should be done only via functions later to prevent bugs)

         this.addVote = function(lFrom, lTo)
         {
            if(lTo in self.votes)
            {
                //user got at least one vote already -> add the new vote
                self.votes[lTo].push(lFrom);
            }else
            {
                //user didn't get a vote yet -> add a list with one vote
                self.votes[lTo] = [lFrom];
            }
         };

         //returns a list of votes a certain userid/answerid received
         this.getVotes = function(lUserId)
         {
             if(lUserId in self.votes)
             {
                 return self.votes[lUserId];
             }
             else{
                 return []; //empty list. user never received a vote
             }
         };


        this.resetRoundData = function()
        {
            self.state = SayAnything.GameState.WaitForStart;

            self.judgeUserId = null;
            self.question = null;
            self.questions = [];
            self.answers = {};
            self.judgedAnswerId = null;
            self.votes = {};
            self.roundScore = {};
         };

         this.awardScore = function(lUserId, lPoints)
         {
             if(lUserId in self.roundScore)
             {
                 self.roundScore[lUserId] += lPoints;
             }else{
                 self.roundScore[lUserId] = lPoints;
             }

             if(lUserId in self.totalScore)
             {
                 self.totalScore[lUserId] += lPoints;
             }else{
                 self.totalScore[lUserId] = lPoints;
             }
         };
    };


    /** The states the game can be in.
     *
     */
     SayAnything.GameState = {
            WaitForStart : 0,
            Rules : 1,
            Questioning : 2,
            Answering : 3,
            ShowAnswers : 4,
            JudgingAndVoting : 5,
            ShowWinner : 6,
            ShowScore : 7
        };

//pre build messages content so it is clear what a message is suppose to contain

   //updates view and client after the shared data changed
    SayAnything.Message.SharedDataUpdate = function(lSharedData)
    {
        this.sharedData = lSharedData;
    };
    SayAnything.Message.SharedDataUpdate.TAG = "SayAnything_SharedDataUpdate";

    //message to the view that a controller clicked the start game button
    SayAnything.Message.StartGame = function()
    {
        //no content
    };
    SayAnything.Message.StartGame.TAG = "SayAnything_StartGame";

    //notifying the view that a controller finished loading the game and is ready to process messages
    SayAnything.Message.GameLoaded = function()
    {
        //no content
    };
    SayAnything.Message.GameLoaded.TAG = "SayAnything_GameLoaded";

    //sent after a controller skips rules
    SayAnything.Message.Rules = function()
    {
        //no content
    };
    SayAnything.Message.Rules.TAG = "SayAnything_SkipRules";

    //sent after the judge chooses a question
    SayAnything.Message.Question = function(lQuestion)
    {
        this.question = lQuestion;
    };
    SayAnything.Message.Question.TAG = "SayAnything_Question";

    //sent after the controllers enter an answer and press the confirm button
    SayAnything.Message.Answer = function(lAnswer)
    {
        this.answer = lAnswer;
    };
    SayAnything.Message.Answer.TAG = "SayAnything_Answer";


    SayAnything.Message.Judge = function(lPlayerId)
    {
        this.playerId = lPlayerId;
    };
    SayAnything.Message.Judge.TAG = "SayAnything_Judge";


    SayAnything.Message.Vote = function(lVotePlayerId1, lVotePlayerId2)
    {
        this.votePlayerId1 = lVotePlayerId1;
        this.votePlayerId2 = lVotePlayerId2;
    };
    SayAnything.Message.Vote.TAG = "SayAnything_Vote";











    //gPlatform is a global variable
    /* global gPlatform */

    var gLastSharedData = null;



    // <editor-fold desc="UI refresh and helper">

        function onMessageUi(lTag, lContent, lFrom)
        {

            //used for view and controller -> update the page
            //if the game data changed
            if(lTag == SayAnything.Message.SharedDataUpdate.TAG)
            {
                var updateSharedDataMessage = JSON.parse(lContent);
                //create a new shared data instance
                var sd = new SayAnything.Data.Shared();

                //copy the received data into this instance
                //this allows to access the helper methods in SayAnything.Data.Shared
                //not just the raw data/json object
                $.extend( sd, updateSharedDataMessage.sharedData );

                //call the refresh ui method
                refreshUi(sd);
            }
        }


        //called if a message arrived which caused the game data to change ->
        //refresh to UI (ideally compare it to the last changed data and
        //only refresh the changed ui)


        /** Game data changed -> refresh the UI
         * TODO: make a backup of the old data and show only the differences?
         * (not really needed for controller though)
         *
         *
         * @param {SayAnything.Data.Shared} lSharedData the updated shared data
         *
         */
        function refreshUi(lSharedData)
        {
            //TODO: UI to data connection
            console.debug("UI refresh");

            //hide everything -> show later only what is needed

            $('.debug').attr("hidden", true);
            $('.view').attr("hidden", true);
            $('.hostcontroller').attr("hidden", true);
            $('.controller').attr("hidden", true);
            $('.judgecontroller').attr("hidden", true);


            //hide all states. TODO: remove that later to avoid flickering during state updates
            $('.stateview').attr("hidden", true);


            var ownUserId = gPlatform.getOwnId();
            var ownScore = 0;
            if(ownUserId in lSharedData.totalScore)
            {
                ownScore = lSharedData.totalScore[ownUserId];
            }


            //bugfix: don't refresh the score during "ShowWinner" state
            //the game data contains the newest score already! but
            //it shouldn't be known until the score state
            if(lSharedData.state != SayAnything.GameState.ShowWinner)
                $(".myScore").empty().append(ownScore);

            $('.timeLeft').empty().append(lSharedData.timeLeft);
            //show the correct state view
            if(lSharedData.state == SayAnything.GameState.WaitForStart)
            {
                console.log("show WaitForStart");
                refreshPlayerList();
                $('#WaitForStart').attr("hidden", false);
            }else if(lSharedData.state == SayAnything.GameState.Rules)
            {
                console.log("show Rules");
                $('#Rules').attr("hidden", false);
            }else if(lSharedData.state == SayAnything.GameState.Questioning)
            {
                //clear up answering field for next run
                $('#answer').val("");
                console.log("show Questioning");
                
                questionListFill(lSharedData.questions[0], lSharedData.questions[1], lSharedData.questions[2], lSharedData.questions[3]);
                $('#Questioning').attr("hidden", false);
            }else if(lSharedData.state == SayAnything.GameState.Answering)
            {
                console.log("show Answering");

                if (ownUserId in lSharedData.answers)
                {
                    $('.preAnswer').attr("hidden", true);
                    $('.postAnswer').attr("hidden", false);
                }
                else
                {
                    $('.preAnswer').attr("hidden", false);
                    $('.postAnswer').attr("hidden", true);
                }

                $('#Answering').attr("hidden", false);
            }else if(lSharedData.state == SayAnything.GameState.ShowAnswers)
            {
                console.log("show ShowAnswers");
                $('#ShowAnswers').attr("hidden", false);
                answerListFill(lSharedData);

            }else if(lSharedData.state == SayAnything.GameState.JudgingAndVoting)
            {
                console.log("show JudgingAndVoting");
                $('#JudgingAndVoting').attr("hidden", false);
                answerListFill(lSharedData);


            }else if(lSharedData.state == SayAnything.GameState.ShowWinner)
            {
                console.log("show ShowWinner");
                $('#ShowWinner').attr("hidden", false);
                answerListFill(lSharedData);

            }else if(lSharedData.state == SayAnything.GameState.ShowScore)
            {
                console.log("show ShowScore");
                $('#ShowScore').attr("hidden", false);
                answerListFill(lSharedData);

            }else{
                console.debug("ERROR: GUI doesn't know state " + lSharedData.state);
            }

            if(lSharedData.judgeUserId != null && lSharedData.judgeUserId == gPlatform.getOwnId())
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
                $('.judgeName').append(getPlayerName(lSharedData.judgeUserId));
            }

            //if question is set -> fill in the question parts in the ui
            if(lSharedData.question != null)
                $('.chosenQuestion').empty().append(lSharedData.question);


            gLastSharedData = lSharedData;
        }

        function refreshPlayerList()
        {
            $('#playerlist').empty();

            var lControllers = gPlatform.getControllers();
            for(var id in lControllers )
            {
                $('#playerlist').append( "<li>" + getPlayerName(id) + "</li>" );
            }
        }
        /**
         * Returns the player name if available
         *
         * @param {type} lUserId the user id
         * @returns {String} the players name or "disconnected" if not available
         */
        function getPlayerName(lUserId)
        {
            var playerName = "user " + lUserId;
            if(typeof gPlatform === "undefined" || gPlatform === null)
            {
                return playerName;
            }
            var controllers = gPlatform.getControllers();
            if(lUserId in controllers)
            {
                var controller = controllers[lUserId];
                playerName = controller.getName();
            }
            return playerName;
        }
        /** special method only called while entering the question state. fill only the question list
         *
         */
        function questionListFill(lQ1, lQ2, lQ3, lQ4)
        {
            $("input[name=questionList]").each(function (index, val)
            {
                $(this).prop("checked", false);
            });

            $("#questionListQ1").val(lQ1);
            $("#questionListQ2").val(lQ2);
            $("#questionListQ3").val(lQ3);
            $("#questionListQ4").val(lQ4);
            $("#questionListQ1Label").empty().append(lQ1);
            $("#questionListQ2Label").empty().append(lQ2);
            $("#questionListQ3Label").empty().append(lQ3);
            $("#questionListQ4Label").empty().append(lQ4);
            $("#questionCustom").val("");
        }
        //fills the list of answers into the GUI elements
        function answerListFill(lSharedData)
        {
            $(".chosenQuestion").empty().append(lSharedData.question);

            //this loop will go through all given answers and fill the UI with data (answers, id's of checkboxes/radio buttons, votes for the answers, player names that received and gave votes)
            var counter = 1;
            for(var userId in lSharedData.answers)
            {

            //answers
                var parentElement = ".A" + counter;
                //Label is in all constructs the parent -> make it visible for this answer
                $(parentElement).attr("hidden", false);

                 $(parentElement + " .playerName").empty().append(getPlayerName(userId));
                //A1, A2 and so on are used for spans that are suppose to contain the answers -> add them
                $(parentElement + " .answer").empty().append(lSharedData.answers[userId]);

                //making sure previously selected answers are unchecked in a new round
                for(var i = counter; i <= 9; i++) //10 = max player numbers
                {
                    $(".judgeList " + parentElement + " #jr" + i).prop("checked", false);
                    $(".voteList " + parentElement + " #vr1" + i).prop("checked", false);
                    $(".voteList " + parentElement + " #vr2" + i).prop("checked", false);
                }


                //set the id in the value attribute of checkboxes. we use that later to find out who the vote/judge belongs to
                $(parentElement + " .CB").val(userId);

            //votes
                var votes = lSharedData.getVotes(userId);
                var votesParentTag = $(parentElement + " .votes");
                votesParentTag.empty();
                votesParentTag.append("Voted by ");
                for(var i = 0; i < votes.length; i++)
                {
                    votesParentTag.append(getPlayerName(votes[i]) + " ");
                }

                //if this is the selected answer by the judge -> add the judge mark
                if(lSharedData.state == SayAnything.GameState.ShowWinner && userId == lSharedData.judgeAnswerId)
                {
                    votesParentTag.append(" <b>" + getPlayerName(lSharedData.judgeUserId) + "</b>");
                }
                $(parentElement + " .vplayer").empty().append(getPlayerName(userId));

                counter++;
            }


            //hide all UI slots that aren't filled with answers
            for(var i = counter; i <= 9; i++) //10 = max player numbers
            {
                var parentElement = ".A" + i;
                //Label is in all constructs the parent -> make it visible for this answer
                $(parentElement).attr("hidden", true);
            }
        }




    // </editor-fold>


    /*
     * Handlers of the UI. Note: They are all for controllers only as the
     * bigview doesn't support any input.
     * As the logic itself is on the bigview all these methods do is
     * sending out messages to the view. The view might change its state and
     * respond with an game data update which causes the refreshUi function
     * to be called. The view can also just ignore the input.
     */
    // <editor-fold desc="UI event handlers">

        /**Controller clicks the start game button
         *
         */
        function startGame()
        {
            gPlatform.sendMessageObj(SayAnything.Message.StartGame.TAG, new SayAnything.Message.StartGame());
        }


        /**Controller clicks the leave game button
         */
        function leaveGame()
        {
            gPlatform.enterGame("gamelist");
        }

        function skipRules()
        {
            gPlatform.sendMessageObj(SayAnything.Message.Rules.TAG, new SayAnything.Message.Rules());
        }

        /**Confirm button of the question input
         */
        function questionListConfirm()
        {
            var question = $("input[name=questionList]:checked").val();
            if(question == "custom")
            {
                question = $("#questionCustom").val();
            }
            //alert(question);
            gPlatform.sendMessageObj(SayAnything.Message.Question.TAG, new SayAnything.Message.Question(question));
        }


        /**Confirm button of the answer list
         *
         * @returns {undefined}
         */
        function answerListConfirm()
        {
            $('.preAnswer').attr("hidden", true);
            $('.postAnswer').attr("hidden", false);

            var answer = $("#answer").val();
            gPlatform.sendMessageObj(SayAnything.Message.Answer.TAG, new SayAnything.Message.Answer(answer));
        }

        /**Confirm button of the judge input
         */
        function judgingConfirm()
        {
            var playerid = $("input[name=judgingRadio]:checked").val();
            gPlatform.sendMessageObj(SayAnything.Message.Judge.TAG, new SayAnything.Message.Judge(playerid));
        }

        /**Confirm button for the votes
         */
        function votingConfirm()
        {
             var playerid1 = $("input[name=votingRadio1]:checked").val();
             var playerid2 = $("input[name=votingRadio2]:checked").val();
            gPlatform.sendMessageObj(SayAnything.Message.Vote.TAG, new SayAnything.Message.Vote(playerid1, playerid2));
        }
    // </editor-fold>



    /**For testing only!
     * This method can fill shared data and trigger an ui refresh thus
     * allowing to test the UI without having to initialize the whole
     * network
     */
    function initTestUi()
    {

        //pure testdata for offline testing!

        //test data
        var testSharedData = new SayAnything.Data.Shared();
        testSharedData.judgeUserId = 2;
        testSharedData.question = "Random question asked by user 2";
        testSharedData.answers[5] = "answer of user 5";
        testSharedData.answers[6] = "answer of user 6";
        testSharedData.answers[7] = "answer of user 7";
        testSharedData.votes[5] = [6, 6];
        testSharedData.votes[6] = [5, 7];
        testSharedData.votes[7] = [7, 5];
        testSharedData.judgedAnswerId = 5;

        testSharedData.roundScore[2] = 1;
        testSharedData.roundScore[5] = 2;
        testSharedData.roundScore[6] = 0;
        testSharedData.roundScore[7] = 3;
        testSharedData.totalScore[2] = 30;
        testSharedData.totalScore[5] = 40;
        testSharedData.totalScore[6] = 50;
        testSharedData.totalScore[7] = 60;


        answerListFill(testSharedData);
    }











function GetRandomQuestion()
{
    var randomIndex = Math.floor((Math.random() * gQuestions.length));

    return gQuestions[randomIndex];
}
