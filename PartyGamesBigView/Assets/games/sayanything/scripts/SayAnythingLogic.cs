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
        /// <summary>
        /// Must be the same as the scene name + folder name in unity and the same folder name of the controller!
        /// </summary>
        public static readonly string GAME_NAME = "sayanything";


        private SharedData mData = new SharedData();
        // Use this for initialization
        void Start()
        {

            //mData.state = GameState.Voting;
            //mData.votes[1] = new List<int>();
            //mData.votes[1].Add(2);
            //mData.votes[1].Add(3);
            //string json = JsonWrapper.ToJson(mData);

            //SharedData result = JsonWrapper.FromJson<SharedData>(json);

            PPlatform.Instance.Message += OnMessage;
            PPlatform.Instance.GameLoaded(GAME_NAME);
        }

        void OnDestroy()
        {
            if (PPlatform.Instance != null)
                PPlatform.Instance.Message -= OnMessage;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("state:" + mData);
            GUILayout.EndHorizontal();
        }


        public void OnMessage(string lTag, string lContent, int lConId)
        {
            if(lTag == Message.GameLoaded.TAG)
            {
                //a controller joined the game and loaded the game controller code -> send a data update
                //TODO: send the refresh just to the sender?
                RefreshState();
            }else if(lTag == Message.StartGame.TAG)
            {
                //controller send the start game event.
                //TODO: check for player count and send back an error if there aren't enough player
                //OR hide the button until there are enough player (risky though because it could change until the message arrives)
                SwitchState(GameState.Questioning);
            
            }else if(lTag == Message.Question.TAG)
            {
                Question questionMsg = JsonWrapper.FromJson<Question>(lContent);
                mData.question = questionMsg.question;
                SwitchState(GameState.Answering);
            }
            else if (lTag == Message.Answer.TAG)
            {
                Answer answerMsg = JsonWrapper.FromJson<Answer>(lContent);

                mData.answers[lConId] = answerMsg.answer;


                //TODO: add timer and there could be answers that are from users that logged out by now...
                if(mData.answers.Count == PPlatform.Instance.Controllers.Count -1)
                {
                    EnterStateJudgingAndVoting();
                }
            }
            else if (lTag == Message.Judge.TAG)
            {
                Judge judgeMsg = JsonWrapper.FromJson<Judge>(lContent);
                mData.judgedAnswerId = judgeMsg.playerId;

                if(IsJudgeAndVotingFinished())
                {
                    SwitchState(GameState.ShowWinner);
                }
            }
            else if (lTag == Message.Vote.TAG)
            {
                Vote voteMsg = JsonWrapper.FromJson<Vote>(lContent);
                mData.AddVote(lConId, voteMsg.votePlayerId1);
                mData.AddVote(lConId, voteMsg.votePlayerId2);

                if (IsJudgeAndVotingFinished())
                {
                    SwitchState(GameState.ShowWinner);
                }
            }
        }


        public bool IsJudgeAndVotingFinished()
        {
            int votes = 0;

            foreach(var voteList in mData.votes)
            {
                votes += voteList.Value.Count;
            }

            if(votes >= 2 * (PPlatform.Instance.Controllers.Count - 1) && mData.judgedAnswerId != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// TODO: add a "CanSwitch" state before for sanity checking
        /// </summary>
        /// <param name="lTargetGameState"></param>
        public void SwitchState(GameState lTargetGameState)
        {
            Debug.Log("Change state from " + mData.state + " to " + lTargetGameState);
            if (lTargetGameState == GameState.Questioning && mData.state == GameState.WaitForStart
                ||
                lTargetGameState == GameState.Questioning && mData.state == GameState.ShowScore)
            {

                //TODO: at least 2 for testing (ideally 3) players needed
                //

                EnterStateQuestioning();
            }
            else if (lTargetGameState == GameState.Answering && mData.state == GameState.Questioning)
            {
                EnterStateAnswering();
            }
            else if (lTargetGameState == GameState.JudgingAndVoting && mData.state == GameState.Answering)
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
            RefreshState();
        }

        private void EnterStateAnswering()
        {

            mData.state = GameState.Answering;
            RefreshState();
        }


        private void EnterStateJudgingAndVoting()
        {
            mData.state = GameState.JudgingAndVoting;
            RefreshState();
        }


        private void EnterStateShowWinner()
        {
            mData.state = GameState.ShowWinner;
            CalculateRoundScore();
            RefreshState();
            StartCoroutine(CoroutineSwitchToShowScore());
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


        private IEnumerator CoroutineSwitchToShowScore()
        {
            yield return new WaitForSeconds(10);
            SwitchState(GameState.ShowScore);
        }


        private void EnterStateShowScore()
        {
            mData.state = GameState.ShowScore;
            CalculateFinalScore();
            RefreshState();
            StartCoroutine(CoroutineSwitchToShowQuestioning());
        }
        private IEnumerator CoroutineSwitchToShowQuestioning()
        {
            yield return new WaitForSeconds(10);
            SwitchState(GameState.Questioning);
        }

        /// <summary>
        /// Gets a random user id from the controller list. 
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetRandomUserId()
        {
            int count = PPlatform.Instance.Controllers.Count;

            int randomIndex = UnityEngine.Random.Range(0, count - 1);
            

            var en = PPlatform.Instance.Controllers.OrderBy<KeyValuePair<int, Controller>, int>((v) => v.Value.Id);
            int nm = 0;
            foreach(var v in en)
            {
                if (nm == randomIndex)
                    return v.Value.Id;
                nm++;
            }

            return -1;
        }

        private int GetNextJudgeId(int last)
        {
            var en = PPlatform.Instance.Controllers.OrderBy<KeyValuePair<int, Controller>, int>((v) => v.Value.Id);

            bool lastFound = false;
            int next = -1;
            foreach(var v in en)
            {
                if(lastFound)
                {
                    next = v.Value.Id;
                    break;
                }
                if(v.Value.Id == last)
                {
                    lastFound = true;
                }
            }


            //the last user was the last one in the list? restart with the first
            if(lastFound && next == -1)
            {
                if(en.Any())
                {
                    next = en.First().Value.Id;
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
            PPlatform.Instance.Send(SharedDataUpdate.TAG, json);
        }
    }

}
