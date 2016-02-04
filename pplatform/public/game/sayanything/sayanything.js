
    //global variables
    var gLogic = null;
    var gPlatform = null;
    

    //called once after the page is loaded see index.html
    function initGame()
    {
        //Setup of the platform and UI
        if(typeof parent.gPlatform !== 'undefined')
        {
            gPlatform = parent.gPlatform;
            
            
            
            console.log("Game listener added");
            if(gPlatform.isView() == false)
            {
                gPlatform.sendMessage(SayAnything.Message.GameLoaded.TAG, new SayAnything.Message.GameLoaded());
            }
            
            if(gPlatform.isView())
            {
                gLogic = new SayAnything.Logic();
                gLogic.init(); //make sure gPlatform is ready before this call
            }
        }
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
            Questioning : 1,
            Answering : 2,
            ShowAnswers : 3,
            Judging : 4,
            Voting : 5,
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
    
    
    //var startGameMessage = new SayAnything.Message.StartGame();
    //alert(JSON.stringify(startGameMessage));
    
    
    
//Game logic
    /**
     * Will construct the Say Anything game logic.
     * 
     */
    SayAnything.Logic = function()
    {
        var mData = new SayAnything.Data.Shared();
        this.getSharedData = function(){return mData;}
        
        //used to keep track who voted already
        var mVoted = {};
        
        /**
         * Gets the logic ready. After this call the logic will react
         * to messages received via the platform and will send out
         * messages itself.
         */
        this.init = function()
        {
            //add the message listener
            gPlatform.addMessageListener(gLogic.onMessage);
            refreshState();
        }
        
        this.onMessage = function(lTag, lContent, lFrom)
        {
            //game messages for the view 
            if(lTag == SayAnything.Message.StartGame.TAG)
            {
                console.log("start game");
                startNewRound();   
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
                    mVoted = {};
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
                    if(lFrom in mVoted)
                    {
                        //already an answer received. ignored
                        console.debug("already an votes received. ignored " + lContent.answer);
                    }else{
                        
                        mVoted[lFrom] = true;
                        mData.addVote(lFrom, lContent.votePlayerId1);
                        mData.addVote(lFrom, lContent.votePlayerId2);
                        

                        
                        if(Object.keys(mVoted).length >= Object.keys(gPlatform.getControllers()).length - 1)
                        {
                            //received same amount of votes as controllers available -> next state
                            calculateScore();
                            mData.state = SayAnything.GameState.ShowWinner;
                            setTimeout(function()
                            {
                                mData.state = SayAnything.GameState.ShowScore;
                                refreshState();
                                setTimeout(function()
                                {
                                    //showed the score for 10 sec. start a new round
                                    startNewRound();
                                }, 10000);
                            }, 10000);
                        }
                        
                        refreshState();
                    }
                }else{
                    console.debug("Received an answer during invalid state " + mData.state);
                }
            }
        }
        
        
        
        //called either after the last show score screen or after the start game button was pressed
        function startNewRound()
        {
            console.log("start new round");
            
            var lastUser = mData.judgeUserId;
            
            mData.resetRoundData();
            if (typeof lastUser === 'undefined')
            {
                //no last user set. -> chose one user randomly
                mData.judgeUserId = getRandomPlayerId();
            }else
            {
                //search next key in the list
                var keys = Object.keys(gPlatform.getControllers());
                var newUser;
                var found = false;
                for(var i = 0; i < keys.length - 1; i++)
                {
                    //looking for the id that is bigger then the last user id but the smallest possible one
                    
                    if(lastUser == keys[i])
                    {
                        //next bigger user id found -> 
                        newUser = keys[ i + 1];
                        found = true;
                    }
                }
                //not found or restarting the round from 0
                if(found == false)
                {
                    newUser = keys[0];
                }
                mData.judgeUserId  = newUser;
            }
            
            
            mData.state = SayAnything.GameState.Questioning;
            refreshState();
        }
        
        function getRandomPlayerId()
        {
            var keys = Object.keys(gPlatform.getControllers());
            var count = keys.length;
            var rand = Math.floor(Math.random() * (count));
            return keys[rand];
        }
        
        //Will be called before showing the score screen
        function calculateScore()
        {
            //1 point is given to the player that wrote the selected answer.
            mData.awardScore(mData.judgedAnswerId, 1); //judgedAnswerId == userid of the user that gave the answer
            
            //The Judge gets 1 point for each Player Token placed on the answer she selected (Max 3 points.)
            var selectedVotes = mData.getVotes(mData.judgedAnswerId);
            var selectedAnswerVoteCount = selectedVotes.length;
            if(selectedAnswerVoteCount > 3)
                selectedAnswerVoteCount = 3;
            mData.awardScore(mData.judgeUserId, selectedAnswerVoteCount);
        
            
            //Players get 1 point for each player Token they placed on the answer that was selected.
            //go trough the vote list of the selected answer. the votes are the userid's of the people who voted -> they get a point
            for(var i = 0; i < selectedVotes.length; i++)
            {
                mData.awardScore(selectedVotes[i], 1);
            }
        }
        
        function refreshState()
        {
            gPlatform.sendMessage(SayAnything.Message.SharedDataUpdate.TAG , new SayAnything.Message.SharedDataUpdate(mData));
        }
        
    };
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    

 
    
    


    

    
    



var gQuestions =
[
"When would be the worst time to burst out laughing?", 
"What's the best song that has been used in a movie?", 
"Who's the most memorable book character?", 
"What's the most underated place for a date?", 
"What would be the most likely reason for me to end up in jail?", 
"What would be the best store to work at?", 
"What's the cheeist pop song ever?", 
"What historical event would have been the coolest to witness in person?", 
"What's the most important aspect of a good relationship?", 
"Who would you be the least surprised to find out is an alien?", 
"What's the best dessert of all time?", 
"What's the worst show currently on TV?", 
"Which famous person would be the most comical as a star of a musical?", 
"What would be the best company to work for?", 
"What's the dumbest thing that someone has actually done?", 
"What would I do if I inherited $100,000?", 
"What's the best cable TV channel?", 
"Who's the greatest painter of all time?", 
"What's the best present to get for a significant other?", 
"Who would be the worst person to sit next to on an airplane?", 
"What would be the worst pet to have?", 
"If I had to watch one movie every day for a year, what movie would I choose?", 
"If I ran my own country, what would be the first law I enact?", 
"What would be the most fun thing to smash with a giant hammer?", 
"What Guinnes world record would my mother least want me to attempt?", 
"What was the most fun thing to do during school recess?", 
"What's the funniest TV comedy skit?", 
"What's the most boring book of all time?", 
"What's the best thing about money?", 
"What would be the oddest subject for a documentary?", 
"What dance would I most want to be good at?", 
"What's the scariest movie ever?", 
"Who's the most annoying person in show business?", 
"What's the worst thing about being a man?", 
"What should my gravestone say?", 
"What's the best candy of all time?", 
"What's the best TV game show of all time?", 
"What would be the weridest New Year's resolution?", 
"What's the best pizza topping?", 
"Who would be the worst movie character to be?", 
"Who was the most inspirationl figure of the past 500 years?", 
"What would be the most inappropriate item to bring to show & tell?", 
"What's the most important household item?", 
"What's the best animated TV show of all time?", 
"Who would be the coolest person to trade places with?", 
"What's the sexiest personality trait for a woman?", 
"What would be the grossest thing to kiss?", 
"What's the most romantic place for a honeymoon?", 
"What's the best movie to randomly catch on TV?", 
"What's the best places to get the news?", 
"What's the most annoying thing about being a woman?", 
"Where would be the worst place to wake up?", 
"Which technology produce would be the hardest to live without?", 
"What's the best movie sequeal of all time?", 
"Who's the best dressed celebrity?", 
"Who was the most controversial figure of the past 50 years?", 
"What would be the weridest fortune to find in a fortune cookie?", 
"What's the best way to pamper yourself?", 
"Who's the best looking acctress of all time?", 
"What famous person should never be allowed to rap?", 
"What would be the best topic for a college class?", 
"If I was invisible for a day, what would I do?", 
"Which country would be the most intereing to travel to?", 
"Who's the most memorable movie character ever?", 
"What's the best fashion trend of all time?", 
"What company would I most want to run?", 
"What would be the worst thing to have in your mouth?", 
"What toy is the most fun for adults to play with? ", 
"What's the most memorable movie line ever?", 
"Which celebrity would make the best spouse?", 
"What is my most valuable possession?", 
"If you could be the opposite gender for a day, what would you do?", 
"If I could win a $1,000 gift card for any store, which would it be?", 
"Which TV character would I most want to be?", 
"Which historical figure would be the most interesting to have dinner with?", 
"What's the best thing about college?", 
"I just moved. What's the first thing I do?", 
"What would I do if I never had to work for a living?", 
"What movie or TV show quote is the most fun to say?", 
"What single item would I put in a time capsule to be opened in the year 3000?", 
"What would be the worst thing to be trapped in an elevator with?", 
"What's the best excuess for forgetting an anniversay?", 
"What gift would I be most surprised to recieve for my birthday?", 
"Which politician, past or present, would make the greatest super villian?", 
"If I could bring one person back to life, who would I choose?", 
"What's the worst thing my neighbors could catch me doing?", 
"What's the most embarassing thing that could happen on a blind date?", 
"What city would be the most fun to live in?", 
"What's the best animated movie of all time?", 
"Which athlere would be the most fun to be?", 
"What's the one thing I would most want to do before I die?", 
"I just got to Las Vegas. What's the first thing I do?", 
"What would be the coolest thing to collect?", 
"What's the most annoying song on the radio?", 
"What's the funniest TV show of all time?", 
"What's the most important quality a person can have?", 
"Who would be the least inspring motivational speaker?", 
"What would be the worst possible pizza topping?", 
"Who's the greatest villian of all time?", 
"What's the most fun song to sing at a karoake party?", 
"I've been voted the world's best parent. Why?", 
"What would be the dumbest gift to take from a stranger?", 
"If I could be holding anything in my hands right now, what would it be?", 
"Which two people (real or fictional) would you most like to see fight each other?", 
"Which fictional character do I most wish actually existed?", 
"What is the worst idea for a themed wedding?", 
"What would be the worst thing to have thrown in my face?", 
"What's the most delcicious ice cream flavor?", 
"What's the best music album of all time?", 
"What's the best Saturday morning cartoon ever?", 
"What was the most important invention of the past 100 years?", 
"What illegal thing would be the most fun to do if it were legal?", 
"Where's the best place to hang out when you're in high school?", 
"What's the best action movie of all time?", 
"What's the weirdest fad of all time?", 
"What's the most nostalgic thing about being a kid?", 
"What would be the weirdest thing to find written on a bathroom stall?", 
"What would be the worst job to have?", 
"Who's the most overrated actress of all time?", 
"What's the lamest newspaper comic strip?", 
"What's the sexiest personality trait for a man?", 
"What would'nt I want my taxi driver to say?", 
"Which would be the most fun to visit?", 
"What TV theme song is the most fun to wing with friends?", 
"What's the best breakfast cereal?", 
"What makes people happy?", 
"What would be the dumbest thing to say in a job interview?", 
"What's the most refresing drink?", 
"What's the best TV drama of all time?", 
"What's the best oylmpic sport?", 
"Who was the most important person of the past 100 years?", 
"What's my biggest pet peeve?", 
"What's the coolest new technology?", 
"What's the best brand name of all time?", 
"Who's the craziest celebrity?", 
"What's the best way to impress a man?", 
"What would be the weirdest job to get hired for?", 
"What's the best excusee to give when you did'nt finish an assignment?", 
"What theme song should play when I enter a room?", 
"What's the last thing I want to find at home after a vacation?", 
"What would be the coolest robotic attachment to add to my body?", 
"What does santa do every day of the year other than christmas?", 
"What would be the most fun activity to do on the moon?", 
"What song is the most likely to pack a dance floor?", 
"Who's the greatest athlete of all time?", 
"What am I most likely to become famous for?", 
"What do I wish was available for \"check out\" at the library?", 
"What animal would be the most fun to be?", 
"What would be the coolest name for a brand?", 
"Who's the best character from Sesame Street or The Muppets?", 
"What personal quality is the biggest turn off?", 
"What newspaper headline would I most like to see?", 
"What would be the finnest food to throw in a friend's face?", 
"Who's the best looking actor of all time?", 
"What should the goverment spend money on?", 
"What do zoo animals do when people go home?", 
"Who's the best looking actor of all time?", 
"What's the most overrated movie of all time?", 
"What organization would we be worse off without?", 
"What's the worst thing about money?", 
"The world will end in one week. What should I do?", 
"What's the greatest board game of all time?", 
"What would be the best job to have?", 
"Who's the greatest musician or band of all time?", 
"Which store is the most fun to shop in?", 
"What's the most interesting field of study?", 
"What's going through my head right now?", 
"Which tourist attraction is most worth visting?", 
"What's the best Tom Cruise movie?", 
"What's the tackiest thing that people do?", 
"What would be the best wedding present?", 
"What technology don't we have that you wish we did?", 
"What's the most fun sport to play?", 
"What's the most suspensful movie ever?", 
"What's the most nostalgic childhood song?", 
"What hobby would I most like to take up?", 
"What would be the worst to scream during church?", 
"What word is the most fun to say out loud?", 
"What's the best Beatles song?", 
"What's the best way to spend a Saturday night?", 
"What am I most likely to be doing in 20 years?", 
"What should'nt be done while driving?", 
"What's my biggest guilty pleasure?", 
"What's the most memorable TV commercial ever?", 
"Which celebrity would be the most fun to hang out with?", 
"What would be the best era to live in?", 
"What would make work more fun?", 
"What job would I most like to try for a week?", 
"What's the most fun sport to watch on TV?", 
"Who's the greatest author of all time?", 
"What's the most important issue facing our nation today?", 
"An alien ship landed on Earth. What should we do?", 
"What's the best way to waste time?", 
"What TV show is the guiltitiest pleasure?", 
"Who's the world's biggest knucklehead?", 
"What would be the most romantic Valentine's Day gift?", 
"What would be the dumbest thing to do in public?", 
"Where's the best place to go for spring break?", 
"What's the most annoying commercial of all time?", 
"What's the best musical of all time?", 
"What's the greatest thing about living in the country?", 
"What's the dumbest thing to try to do in the dark?", 
"What's the best Halloween costume?", 
"What's the best drama currently on TV?", 
"If I could be anyone famous, would would I choose?", 
"What should more people pay attention to?", 
"Who's the man with the master plan?", 
"What's the best holiday?", 
"Who's the funniest TV character ever?", 
"What would make next weekend more exciting?", 
"What's the best activity for a first date?", 
"A geniue just granted me a wish. What should I ask for?", 
"What's the most delicious fruit?", 
"What's the best song of all time?", 
"If I could go on a date with anyone, who would it be?", 
"What's the ideal romantic evening?", 
"What's the most confusing thing ever?", 
"What would be the greatest world record to hold?", 
"Who's the most overrated actor of all time?", 
"Which celebrity would be the best desert islan companion?", 
"Which historical time period would be the most interesting to visit for a day?", 
"What should always be done by experts?", 
"What would be the coolest thing to do with a $100 million lottery jackpot?", 
"What's the best Tom Hanks movie?", 
"What's the greatest video game of all time?", 
"What organization would we be better off without", 
"What's [PLAYER'S NAME] thinking right now?", 
"What would be a good task for a Boy Scout merit badge?", 
"What's the best date movie of all time?", 
"Who would be the most fun person to watch sing Karaoke?", 
"What should we learn in high school that we don't?", 
"Your parents are out of town. What happens at the party?", 
"What's the great thing about living in a city?", 
"What TV channel would be the hardest to live without?", 
"What's the best sit-down restaurant chain?", 
"What's the meaning of life?", 
"Why did the chicken cross the road?", 
"What would be the coolest thing to have at a mansion?", 
"What would be the coolest TV show to guest star on?", 
"What's the funniest YouTube video?", 
"What interest would I most want my significant other to share?", 
"What's the cheesiest pickup line ever?", 
"What would be the most fun thing to throw off a tall building?", 
"Which movie prop would be the coolest to own?", 
"Which famous person would be the most difficult to have as an in-law?", 
"What's the most important invention of all time?", 
"What would be the most difficult item to sell door-to-door?", 
"Where would be the worst place to live?", 
"What's the worst reality TV show of all time?", 
"Which celebrity would make the worst presidential canidate?", 
"What's the best way to impress a woman?", 
"What would be the weirdest fear to have?", 
"What's the most uselss household item?", 
"What hit song should never have been recorded?", 
"If my life was a movie, what would it be?", 
"What living person would be the coolest to have dinner with?", 
"What's the worst place for a date?", 
"Which animal would make the greatest pet?", 
"What musician or band would be the most embarassing to have in your collection?", 
"Who's the best movie actor of all time?", 
"What one word best describes me?", 
"What's the worst thing to say to a cop after getting pulled over?", 
"What's the most pleasant kitchen aroma?", 
"Who's the best movie couple of all time?", 
"Which famous historical figure would make the best prom date?", 
"Whats the most important quality in a parent?", 
"What really ticks people off?", 
"What would I most want to see constructed out of legos?", 
"What's the best TV show to watch in re-runs?", 
"Who's the most creative artist of all time?", 
"What's the least interesting academic subject?", 
"What's a husband most likely to forget?", 
"What magical power would be the coolest to have?", 
"What's the most romantic movie of all time?", 
"Who was the most important person of the past 10 years?", 
"What's the best way to spend the day when playing hooky?", 
"If I could have a BIG anything, what would it be?", 
"What's the best way to spend a rainy day?", 
"Which TV character would I least want to be?", 
"What's the greatest snack food of all time?", 
"What would be the coolest thing to try just once?", 
"What's the weridest thing that could happen right now?", 
"What's the most relaxing vaction spot?", 
"Who's the best TV couple of all time?", 
"Who would be the most interesting person to take a class from?", 
"What do kids hate most?", 
"Where'es the best place to take off your pants?", 
"What gift is most likely to get regifted?", 
"Who is the most overrated author of all time?", 
"If I could have a private concert featuring anyone, who would it be?", 
"Who would I least want a call from in the middle of the night?", 
"What would be the worst question to ask someone on a first date?", 
"What's the tastiest ethnic cuisine?", 
"Who's the most memorable TV character ever?", 
"What website would be the hardest to live without?", 
"What's the most pressing issue the world will face over the next 50 years?", 
"What does'nt taste better with ketchup?", 
"What's the scariest animal?", 
"What's the best reality TV show of all time?", 
"What's the greatest creative work of all time?", 
"What's the worst habit to have?", 
"What would my pet say about me if it could talk?", 
"What's the tasitest pie flavor?", 
"What movie should never have been made?", 
"What's the worst fashion trend of all time?", 
"What does the world need more of?", 
"What would be the dumbest thing to say to your new mother-in-law?", 
"What is one food I will never try to eat?", 
"What movie absolutely does not need a sequel?", 
"What's the most embarassing thing for a parent to do?", 
"How would you dipose of a mountain of cheese?", 
"If I could tattoo anything on my friend's face, what would it be?", 
"What's the most fun activity to do in your free time?", 
"What's the best superhero movie of all time?", 
"What's the most interesting book of all time?", 
"If I could have anything, what would it be?", 
"What do people say to dogs that you should'nt say to your boss?", 
"What would be the coolest car to own?", 
"What's the funniest show currently on TV?", 
"Which celebrity has no business being a celebrity?", 
"What's the most important thing in life?", 
"If you could train a monkey to do anything, what would it be?", 
"What's the best thing you can buy for five bucks?", 
"What movie is required viewing for all geeks?", 
"What celebrity would make the best nanny?", 
"What's the biggest waste of money?", 
"I just got fired. Why?", 
"Which fast food chain would be the hardest to live without?", 
"What would be the worst movie to get remade with a completely nude cast?", 
"What's the greatest thing about being a celebrity?", 
"I just wrote a book. What's it called?", 
"What would be the most inappropriate thing to have on your desk?", 
"What's the best place to buy shoes?", 
"What's the sappiest love song ever?", 
"What's the most overrated TV show of all time?", 
"What's the best thing about being a man?", 
"Who should just shut up?", 
"What do most kids want to be when they grow up?", 
"What's the funniest newspaper comic strip?", 
"What's the most important quality in a friend?", 
"What would be the weirdest thing to collect?", 
"What's the best place to buy clothes?", 
"What's the funniest movie of all time?", 
"Who's the weirdest celebrity couple ever?", 
"What's the most awkard thing about being in middle school?", 
"What does an ostrich think about when its heads is in the sand?", 
"What's the best spice?", 
"What's the best Eddie Murphy movie?", 
"What would be the worst name for a superhero?", 
"What's the best thing about weddings?", 
"What would be the most inappropriate thing to stay on a first date?", 
"What's the best magazine to take on an airplane flight?", 
"What's the best Brad Pitt movie?", 
"Who's currently the greatest professional athlete?", 
"What's the worst thing about being a woman?", 
"What did I dream about last night?", 
"What's the best beer?", 
"Who's the best movie acctress of all time?", 
"Who was the most important person in history?", 
"What would be the coolest skill to have without needing to pratice?", 
"What would'nt you want to see on a fast food menu?", 
"What would be the most exotic place to travel to?", 
"What's the best movie of all time?", 
"Who's the best cartoon character of all time?", 
"What's the nerdiest occupation?", 
"What would be the weirdest secret to hear about your mother?", 
"What would be the best Mother's Day gift?", 
"Who's the greatest character from a children's television show or movie?", 
"If you had to adopt one historical figure as a baby, who would it be?", 
"What would be the best way to get fired from a job?", 
"What message would I write on the moon for all to see?", 
"What's the most exhausting physical activity?", 
"Which current TV show would be the hardest to live without?", 
"What famous person best defines the word \"couragerous\"?", 
"What would I most like to do after I retire?", 
"What's a good t-shirt slogan?", 
"What do I want most for my next birthday?", 
"What musician would be the most interesting in a talk show interview?", 
"What's the best romantic comedy of all time?", 
"What's the most useless thing students learn in school?", 
"If Ghandi was invisible for the day, what would he do?", 
"What's the most exciting sport to see in person?", 
"Whos' the most overrated band of all time?", 
"Who's the funniest comedian of all time?", 
"What's the most fascinating thing about being human?", 
"What would Jesus do?", 
"What would be the coolest thing to be able to predict?", 
"What's the best dramatic movie of all time?", 
"Who's the coolest superhero?", 
"What's the world's most impressive manmade structure?"
];


function GetRandomQuestion()
{
    var randomIndex = Math.floor((Math.random() * gQuestions.length));
    
    return gQuestions[randomIndex];
}