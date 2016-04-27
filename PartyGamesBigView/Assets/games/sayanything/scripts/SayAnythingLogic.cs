using UnityEngine;
using System.Collections;
using PPlatform.Helper;
using System.Collections.Generic;
using PPlatform.SayAnything.Message;
using System;
using System.Linq;


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
                GUILayout.Label("state:" + mData);
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

            mData.state = GameState.Questioning;
            mData.timeLeft = QUESTIONING_TIME;
            RefreshState();
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
    }

}
