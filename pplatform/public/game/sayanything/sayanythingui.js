

    //gPlatform is a global variable
    /* global gPlatform */
    

    function initUI()
    {
        //fills the ui with test data
        initTestUi();
        
        //all the ui needs is the possibility to receive messages
        //no direct access to the game logic is needed
        gPlatform.addMessageListener(onMessageUi);
        
    }
    

    // <editor-fold desc="UI refresh and helper">

        function onMessageUi(lTag, lContent, lFrom)
        {    

            //if it is running on the view -> check if the controller
            //list was updated
            if(gPlatform.isView())
            {
                //platform specific messages
                if(lTag == TAG.CONTROLLER_DISCOVERY || lTag == TAG.CONTROLLER_LEFT)
                {
                    $('#playerlist').empty();

                    var lControllers = gPlatform.getControllers();
                    for(var id in lControllers )
                    {
                        $('#playerlist').append( "<li>" + lControllers[id].name + "</li>" );
                    }
                }
            }

            //used for view and controller -> update the page
            //if the game data changed
            if(lTag == SayAnything.Message.SharedDataUpdate.TAG)
            {
                //create a new shared data instance
                var sd = new SayAnything.Data.Shared();

                //copy the received data into this instance
                //this allows to access the helper methods in SayAnything.Data.Shared
                //not just the raw data/json object
                $.extend( sd, lContent.sharedData );

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

                questionListFill(GetRandomQuestion(), GetRandomQuestion(), GetRandomQuestion(), GetRandomQuestion());
                $('#Questioning').attr("hidden", false);
            }else if(lSharedData.state == SayAnything.GameState.Answering)
            {
                console.log("show Answering");
                $('#Answering').attr("hidden", false);
            }else if(lSharedData.state == SayAnything.GameState.ShowAnswers)
            {
                console.log("show ShowAnswers");
                $('#ShowAnswers').attr("hidden", false);
                answerListFill(lSharedData);

            }else if(lSharedData.state == SayAnything.GameState.Judging)
            {
                console.log("show Judging");
                $('#Judging').attr("hidden", false);
                answerListFill(lSharedData);

            }else if(lSharedData.state == SayAnything.GameState.Voting)
            {
                console.log("show Voting");
                $('#Voting').attr("hidden", false);
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
                scoreListFill(lSharedData);

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

    //fills the ui with data

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

                 $(parentElement + " .playerName").empty().append("Player " + userId);
                //A1, A2 and so on are used for spans that are suppose to contain the answers -> add them
                $(parentElement + " .answer").empty().append(lSharedData.answers[userId]);

                //set the id in the value attribute of checkboxes. we use that later to find out who the vote/judge belongs to
                $(parentElement + " .CB").val(userId);

            //votes
                var votes = lSharedData.getVotes(userId);
                var votesParentTag = $(parentElement + " .votes");
                votesParentTag.empty();
                votesParentTag.append("Voted by ");
                for(var i = 0; i < votes.length; i++)
                {
                    votesParentTag.append("player " + votes[i] + " ");
                }

                //if this is the selected answer by the judge -> add the judge mark
                if(lSharedData.state == SayAnything.GameState.ShowWinner && userId == lSharedData.judgeAnswerId)
                {
                    votesParentTag.append(" <b>" + "player " + lSharedData.judgeUserId + "</b>");
                }
                $(parentElement + " .vplayer").empty().append("player " + userId);

                counter++;
            }


            //hide all UI slots that aren't filled with answers
            for(var i = counter; i <= 10; i++) //10 = max player numbers
            {
                var parentElement = ".A" + i;
                //Label is in all constructs the parent -> make it visible for this answer
                $(parentElement).attr("hidden", true);
            }
        }


        function scoreListFill(lSharedData)
        {
            //go trough all scores and fill in the user
            var counter = 1;
            for(var userId in lSharedData.totalScore)
            {
                var parentElement = ".U" + counter;
                $(parentElement).attr("hidden", false);

                 $(parentElement + " .playerName").empty().append("Player " + userId);
                if(userId in lSharedData.roundScore){
                     $(parentElement + " .roundScore").empty().append(lSharedData.roundScore[userId]);
                }else{
                    $(parentElement + " .roundScore").empty();
                }
                $(parentElement + " .totalScore").empty().append(lSharedData.totalScore[userId]);
                counter++;
            }
            //hide all UI slots that aren't filled with answers
            for(var i = counter; i <= 10; i++) //10 = max player numbers
            {
                var parentElement = ".U" + i;
                //Label is in all constructs the parent -> make it visible for this answer
                $(parentElement).attr("hidden", true);
            }
        }


        /** special method only called while entering the question state. fill only the question list
         * 
         */
        function questionListFill(lQ1, lQ2, lQ3, lQ4)
        {


            $("#questionListQ1").val(lQ1);
            $("#questionListQ2").val(lQ2);
            $("#questionListQ3").val(lQ3);
            $("#questionListQ4").val(lQ4);
            $("#questionListQ1Label").empty().append(lQ1);
            $("#questionListQ2Label").empty().append(lQ2);
            $("#questionListQ3Label").empty().append(lQ3);
            $("#questionListQ4Label").empty().append(lQ4);
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
            gPlatform.sendMessage(SayAnything.Message.StartGame.TAG, new SayAnything.Message.StartGame());
        }


        /**Controller clicks the leave game button
         */
        function leaveGame()
        {
            gPlatform.enterGame("./gamelist.html");
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
            gPlatform.sendMessage(SayAnything.Message.Question.TAG, new SayAnything.Message.Question(question));
        }


        /**Confirm button of the answer list
         * 
         * @returns {undefined}
         */
        function answerListConfirm()
        {
            var answer = $("#answer").val();
            gPlatform.sendMessage(SayAnything.Message.Answer.TAG, new SayAnything.Message.Answer(answer));
        }

        /**Confirm button of the judge input
         */
        function judgingConfirm()
        {
            var playerid = $("input[name=judgingRadio]:checked").val();
            gPlatform.sendMessage(SayAnything.Message.Judge.TAG, new SayAnything.Message.Judge(playerid));
        }

        /**Confirm button for the votes
         */
        function votingConfirm()
        {
             var playerid1 = $("input[name=votingRadio1]:checked").val();
             var playerid2 = $("input[name=votingRadio2]:checked").val();
            gPlatform.sendMessage(SayAnything.Message.Vote.TAG, new SayAnything.Message.Vote(playerid1, playerid2));
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
        scoreListFill(testSharedData);   
    }