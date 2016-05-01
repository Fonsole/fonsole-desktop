using UnityEngine;
using System.Collections;
using PPlatform.Helper;
using System.Collections.Generic;
using PPlatform.SayAnything.Message;
using System;
using System.Linq;
using Assets;


namespace PPlatform.SayAnything
{


    /// <summary>
    /// 
    /// 
    /// TODO: add a local list of controllers that is ordered. This would allow to handle player that leave more elegantly +
    /// allow to keep additional info like player order
    /// </summary>
    public class SayAnythingLogic : MonoBehaviour
    {
        public static readonly float RULES_TIME = 15;
        public static readonly float QUESTIONING_TIME = 30;
        public static readonly float ANSWERING_TIME = 60;
		public static readonly float DISPLAY_TIME = 5;
        public static readonly float VOTING_TIME = 30;
        public static readonly float SHOWWINNER_TIME = 10;
        public static readonly float SHOWSCORE_TIME = 10;



        /// <summary>
        /// Must be the same as the scene name + folder name in unity and the same folder name of the controller!
        /// </summary>
        public static readonly string GAME_NAME = "sayanything";


        private SharedData mData = new SharedData();

        public SharedData Data
        {
            get { return mData; }
        }

        /// <summary>
        /// Codes used for error checks / checks if states are valid and if switch to the next state is valid too
        /// 
        /// This is used to avoid exceptions which aren't supported on all unity platforms
        /// </summary>
        public enum StatusCode
        {
            Invalid = 0, //no status code was set. usuallt indicates programming error or memory problems
            Ok = 1, //all fine


            //game problems
            NotEnoughPlayers = 100, //user tried to start a game but there aren't enough players or a new round starts but too many players logged out
            NoQuestionAsked = 101, // judge didn't asked a question
            NoJudgeChoice = 102, //judge didn't choose one of the answers -> no score can't be calculated
            NotEnoughAnswers = 103,//there aren't enough answers to keep playing

        }

        // Use this for initialization
        void Start()
        {

            //mData.state = GameState.Voting;
            //mData.votes[1] = new List<int>();
            //mData.votes[1].Add(2);
            //mData.votes[1].Add(3);
            //string json = JsonWrapper.ToJson(mData);

            //SharedData result = JsonWrapper.FromJson<SharedData>(json);

            Platform.Instance.Message += OnMessage;
            Platform.Instance.GameLoaded(GAME_NAME);
            TL.ActivateLog();
            TL.ActivateEditorDirectJump();
            TL.LogTag(this.GetType().Name);
            TL.LogTag(TL.TAG_ERROR);
            TL.LogTag(TL.TAG_WARNING);
            TL.LogTag(TL.TAG_INFO);
            TL.LogTag(this.GetType().Name);
            TL.L("test");
        }

        void OnDestroy()
        {
            if (Platform.Instance != null)
                Platform.Instance.Message -= OnMessage;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            mData.timeLeft -= Time.fixedDeltaTime;

            if(mData.timeLeft <= 0)
            {
                //time ran out
                if (mData.state == GameState.Rules)
                {
                    StatusCode code = CanEnterStateQuestioning();
                    if (code == StatusCode.Ok)
                    {
                        SwitchState(GameState.Questioning);
                    }
                    else
                    {
                        CancelCurrentRound(code);
                    }
                    //start next round
                }
                else if (mData.state == GameState.Questioning)
                {
                    StatusCode code = CanEnterStateAnswering();
                    if (code == StatusCode.Ok)
                    {
                        SwitchState(GameState.Answering);
                    }
                    else
                    {
                        CancelCurrentRound(code);
                    }
                    //start next round
                }
                else if (mData.state == GameState.Answering)
                {
                    StatusCode code = CanEnterStateDisplay();
                    if (code == StatusCode.Ok)
                    {
                        SwitchState(GameState.DisplayAnswers);
                    }
                    else
                    {
                        CancelCurrentRound(code);
                    }
                }
                else if (mData.state == GameState.DisplayAnswers)
                {
                    StatusCode code = CanEnterStateJudgeAndVoting();
                    if (code == StatusCode.Ok)
                    {
                        SwitchState(GameState.JudgingAndVoting);
                    }
                    else
                    {
                        CancelCurrentRound(code);
                    }
                }
                else if (mData.state == GameState.JudgingAndVoting)
                {
                    //game fails if the judge didn't choose anything
                    //but no votes are fine
                    StatusCode code = CanEnterStateShowWinner();
                    if (code == StatusCode.Ok)
                    {
                        SwitchState(GameState.ShowWinner);
                    }
                    else
                    {
                        CancelCurrentRound(code);
                    }
                }
                else if (mData.state == GameState.ShowWinner)
                {
                    SwitchState(GameState.ShowScore);
                }
                else if (mData.state == GameState.ShowScore)
                {
                    if (CanEnterStateQuestioning() == StatusCode.Ok)
                    {
                        SwitchState(GameState.Questioning);
                    }
                    else
                    {
                        SwitchState(GameState.WaitForStart);
                    }
                }
            }

        }


        private bool debugUi = false;
        private void OnGUI()
        {
            GUILayout.BeginVertical();
            debugUi = GUILayout.Toggle(debugUi, "debugui");
            if (debugUi)
            {
                GUILayout.Label("state:" + mData);

                GUILayout.Label("active Players:");
                foreach(var v in Platform.Instance.ActiveControllers)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("    ");
                    GUILayout.Label(v.UserId + "\t" + v.Name);
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// Will cancel the round and either enter questioning state or the wait for start state if there aren't enough players
        /// </summary>
        private void CancelCurrentRound(StatusCode reason)
        {
            Debug.Log("Status code: " + reason);
            if (CanEnterStateQuestioning() == StatusCode.Ok)
            {
                SwitchState(GameState.Questioning);
            }
            else
            {
                SwitchState(GameState.WaitForStart);
            }
            
        }

        private bool HasEnoughPlayers()
        {
            int activePlayers = 0;
            
            foreach(var v in Platform.Instance.Controllers)
            {
                if(v.Value.IsAvailable)
                    activePlayers++;
            }
            return activePlayers >=3;
        }

        public void OnMessage(string lTag, string lContent, int lConId)
        {
            if(lTag == Message.GameLoaded.TAG)
            {
                //a controller joined the game and loaded the game controller code -> send a data update
                //TODO: send the refresh just to the sender?
                RefreshState();
            }


            if (mData.state == GameState.WaitForStart && lTag == Message.StartGame.TAG)
            {
                //controller send the start game event.
                //TODO: check for player count and send back an error if there aren't enough player
                //OR hide the button until there are enough player (risky though because it could change until the message arrives)
                SwitchState(GameState.Rules);

            }
            else if (mData.state == GameState.Rules && lTag == Message.Rules.TAG)
            {
                SwitchState(GameState.Questioning);
            }
            else if (mData.state == GameState.Questioning && lTag == Message.Question.TAG)
            {
                Question questionMsg = JsonWrapper.FromJson<Question>(lContent);
                mData.question = questionMsg.question;
                SwitchState(GameState.Answering);
            }
            else if (mData.state == GameState.Answering && lTag == Message.Answer.TAG)
            {
                Answer answerMsg = JsonWrapper.FromJson<Answer>(lContent);

                mData.answers[lConId] = answerMsg.answer;


                //TODO: add timer and there could be answers that are from users that logged out by now...
                if(mData.answers.Count == Platform.Instance.Controllers.Count -1)
                {
					EnterStateDisplay();
                }
            }
            else if (mData.state == GameState.JudgingAndVoting && lTag == Message.Judge.TAG)
            {
                Judge judgeMsg = JsonWrapper.FromJson<Judge>(lContent);
                mData.judgedAnswerId = judgeMsg.playerId;

                if(IsJudgeAndVotingFinished())
                {
                    SwitchState(GameState.ShowWinner);
                }
            }
            else if (mData.state == GameState.JudgingAndVoting && lTag == Message.Vote.TAG)
            {
                Vote voteMsg = JsonWrapper.FromJson<Vote>(lContent);
                mData.CancelVotesBy(lConId);
                mData.AddVote(lConId, voteMsg.votePlayerId1);
                mData.AddVote(lConId, voteMsg.votePlayerId2);

                if (IsJudgeAndVotingFinished())
                {
                    SwitchState(GameState.ShowWinner);
                }
            }
            else
            {
                //Debug.LogWarning("Ignored invalid message during state:" + mData.state + " TAG:" + lTag + " content:" + lContent + " from:" + lConId);
            }
        }


        public bool IsJudgeAndVotingFinished()
        {
            int votes = 0;

            foreach(var voteList in mData.votes)
            {
                votes += voteList.Value.Count;
            }

            if(votes >= 2 * (Platform.Instance.Controllers.Count - 1) && mData.judgedAnswerId != SharedData.UNDEFINED)
            {
                return true;
            }
            return false;
        }


        private StatusCode CanEnterStateRules()
        {
            if (HasEnoughPlayers() == false)
                return StatusCode.NotEnoughPlayers;

            return StatusCode.Ok;
        }
        private StatusCode CanEnterStateQuestioning()
        {
            if (HasEnoughPlayers() == false)
                return StatusCode.NotEnoughPlayers;

            return StatusCode.Ok;
        }
        private StatusCode CanEnterStateAnswering()
        {
            if (String.IsNullOrEmpty(mData.question))
                return StatusCode.NoQuestionAsked;

            return StatusCode.Ok;
        }
		private StatusCode CanEnterStateDisplay()
		{
			if (mData.answers.Count < 1) //TODO: <= in the future. at least 2 answers needed but 1 is enough for testing
				return StatusCode.NotEnoughAnswers;

			return StatusCode.Ok;
		}
        private StatusCode CanEnterStateJudgeAndVoting()
        {
            if (mData.answers.Count < 1) //TODO: <= in the future. at least 2 answers needed but 1 is enough for testing
                return StatusCode.NotEnoughAnswers;

            return StatusCode.Ok;
        }
        private StatusCode CanEnterStateShowWinner()
        {
            if (mData.judgedAnswerId == -1)
                return StatusCode.NoJudgeChoice;

            return StatusCode.Ok;
        }
        /// <summary>
        /// TODO: add a "CanSwitch" state before for sanity checking
        /// </summary>
        /// <param name="lTargetGameState"></param>
        public void SwitchState(GameState lTargetGameState)
        {
            Debug.Log("Change state from " + mData.state + " to " + lTargetGameState);

            //questening state can always be entered no matter from which state (the whole round will be cancelt if ShowScore wasn't reached)
            if (lTargetGameState == GameState.WaitForStart)
            {
                AudioManager.Instance.OnWaitForStart();
                EnterStateWaitForStart();
            }
            else if (lTargetGameState == GameState.Rules)
            {
                //AudioManager.Instance.OnStartGame();
                EnterStateRules();
            }
            else if (lTargetGameState == GameState.Questioning)
            {

                //TODO: at least 2 for testing (ideally 3) players needed
                //
				AudioManager.Instance.OnStartGame();
                EnterStateQuestioning();
            }
            else if (lTargetGameState == GameState.Answering && mData.state == GameState.Questioning)
            {
                AudioManager.Instance.OnQuestionSelected();
                EnterStateAnswering();
            }
			else if (lTargetGameState == GameState.DisplayAnswers && mData.state == GameState.Answering)
            {
                EnterStateDisplay();
            }
			else if (lTargetGameState == GameState.JudgingAndVoting && mData.state == GameState.DisplayAnswers)
			{
				EnterStateJudgingAndVoting();
			}
            else if (lTargetGameState == GameState.ShowWinner && mData.state == GameState.JudgingAndVoting)
            {
                EnterStateShowWinner();
            }
            else if (lTargetGameState == GameState.ShowScore && mData.state == GameState.ShowWinner)
            {
                EnterStateShowScore();
            }
            else
            {
                Debug.Log("Invalid state change blocked: From " + mData.state + " to " + lTargetGameState);
            }

        }

        
        private void EnterStateWaitForStart()
        {

            mData.resetRoundData();
            mData.state = GameState.WaitForStart;
            RefreshState();
        }
        private void EnterStateRules()
        {
            mData.state = GameState.Rules;
            mData.timeLeft = RULES_TIME;
            RefreshState();
        }
        private void EnterStateQuestioning()
        {
            int lastJudge = mData.judgeUserId;
            mData.resetRoundData();

            if(lastJudge == SharedData.UNDEFINED)
            {
                mData.judgeUserId = GetRandomUserId();
            }
            else
            {
                mData.judgeUserId = GetNextJudgeId(lastJudge);

                //no follow up user found? happens if the user logged out
                if(mData.judgeUserId == -1)
                    mData.judgeUserId = GetRandomUserId();
            }

            mData.questions = new string[4];
            for (int i = 0; i < mData.questions.Length; i++)
            {
                string qst = GetRandomQuestion();
                qst = ReplaceMarkup(qst);
                mData.questions[i] = qst;
            }


            mData.state = GameState.Questioning;
            mData.timeLeft = QUESTIONING_TIME;
            RefreshState();
        }


        /// <summary>
        /// Use this method to add more markup's that replace parts of the question with values from the game logic's data / player data
        /// </summary>
        /// <param name="qst">A question from the given question list</param>
        /// <returns>A customized question based on the games data and the player list</returns>
        private string ReplaceMarkup(string qst)
        {
            string randomPlayerNameMarkup = "[PLAYER'S NAME]";
            qst = qst.Replace(randomPlayerNameMarkup, GetRandomPlayerName());

            //add more here

            return qst;
        }

        /// <summary>
        /// Returns a random name of active players used to fill into questions.
        /// </summary>
        /// <returns> Name of the player </returns>
        private string GetRandomPlayerName()
        {
            string name = "TestName";
            if(PPlatform.Platform.Instance != null)
            {
                int number = Platform.Instance.ActiveControllers.Count();
                int rdIndex = UnityEngine.Random.Range(0, number);
                Controller c = Platform.Instance.ActiveControllers.ElementAt(rdIndex);
                if (c != null)
                    name = c.Name;
            }
            return name;
        }

        private void EnterStateAnswering()
        {
            mData.state = GameState.Answering;
            mData.timeLeft = ANSWERING_TIME;
            RefreshState();
        }


		private void EnterStateDisplay()
		{
			mData.state = GameState.DisplayAnswers;
			mData.timeLeft = DISPLAY_TIME * (Platform.Instance.Controllers.Count - 1);
			RefreshState();
		}

        private void EnterStateJudgingAndVoting()
        {
            mData.state = GameState.JudgingAndVoting;
            mData.timeLeft = VOTING_TIME;
            RefreshState();
        }


        private void EnterStateShowWinner()
        {
            mData.state = GameState.ShowWinner;
            mData.timeLeft = SHOWWINNER_TIME;
            CalculateRoundScore();
            RefreshState();
            //StartCoroutine(CoroutineSwitchToShowScore());
        }

        private void EnterStateShowScore()
        {
            mData.state = GameState.ShowScore;
            mData.timeLeft = SHOWSCORE_TIME;
            CalculateFinalScore();
            RefreshState();
            //StartCoroutine(CoroutineSwitchToShowQuestioning());
        }

        private void CalculateRoundScore()
        {
            //1 point is given to the user that sent the selected answer
            mData.awardRoundScore(mData.judgedAnswerId, 1);

            //The Judge gets 1 point for each Player Token placed on the answer she selected (Max 3 points.)
            List<int> selectedVotes = mData.GetVotes(mData.judgedAnswerId);
            int judgePoints = selectedVotes.Count;
            mData.awardRoundScore(mData.judgeUserId, judgePoints);

            //each player gets a point for each vote that were given to the answer the judge selected
            foreach(int pid in selectedVotes)
            {
                mData.awardRoundScore(pid, 1);
            }
        }

        private void CalculateFinalScore()
        {
            foreach(var v in mData.roundScore)
            {
                mData.awardTotalScore(v.Key, v.Value);
            }
        }


        //private IEnumerator CoroutineSwitchToShowScore()
        //{
        //    yield return new WaitForSeconds(10);
        //    SwitchState(GameState.ShowScore);
        //}

        //private IEnumerator CoroutineSwitchToShowQuestioning()
        //{
        //    yield return new WaitForSeconds(10);
        //    SwitchState(GameState.Questioning);
        //}

        /// <summary>
        /// Gets a random user id from the controller list. 
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetRandomUserId()
        {
            List<Controller> activeControllers = Platform.Instance.Controllers.Values.Where((x) => { return x.IsAvailable; }).ToList();

            int count = activeControllers.Count;

            int randomIndex = UnityEngine.Random.Range(0, count - 1);

            return activeControllers[randomIndex].UserId;
        }
        
        private int GetNextJudgeId(int last)
        {
            List<Controller> activeControllers = Platform.Instance.Controllers.Values.Where((x) => { return x.IsAvailable; }).OrderBy((x) =>{ return x.UserId;}).ToList();

            bool lastFound = false;
            int next = -1;
            foreach (var v in activeControllers)
            {
                if (lastFound)
                {
                    next = v.UserId;
                    break;
                }
                if (v.UserId == last)
                {
                    lastFound = true;
                }
            }


            //the last user was the last one in the list? restart with the first
            if (lastFound && next == -1)
            {
                if (activeControllers.Any())
                {
                    next = activeControllers.First().UserId;
                }
            }
            return next;

        }
        private int GetNextJudgeId_old(int last)
        {
            var en = Platform.Instance.Controllers.OrderBy<KeyValuePair<int, Controller>, int>((v) => v.Value.UserId);

            bool lastFound = false;
            int next = -1;
            foreach(var v in en)
            {
                if(lastFound)
                {
                    next = v.Value.UserId;
                    break;
                }
                if(v.Value.UserId == last)
                {
                    lastFound = true;
                }
            }


            //the last user was the last one in the list? restart with the first
            if(lastFound && next == -1)
            {
                if(en.Any())
                {
                    next = en.First().Value.UserId;
                }
            }

            //if next wasn't found or the list is empty we return -1
            return next;

        }
        
        private void RefreshState()
        {
            SharedDataUpdate msg = new SharedDataUpdate();
            msg.sharedData = mData;
            string json = JsonWrapper.ToJson(msg);
            Platform.Instance.Send(SharedDataUpdate.TAG, json);
        }

        public static string GetRandomQuestion()
        {
            var randomIndex = UnityEngine.Random.Range(0, gQuestions.Length);
            return gQuestions[randomIndex];
        }



        private static string[] gQuestions =
        {
            //this is the only one with a markup so far
            "What's [PLAYER'S NAME] thinking right now?", 
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
        };
    }

}
