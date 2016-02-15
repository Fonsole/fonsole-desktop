using UnityEngine;
using System.Collections;
using PPlatform.SayAnything;
using System;
using System.Linq;
using PPlatform.Helper;

namespace PPlatform.SayAnything.UI
{
    public class SayAnythingUi : UnitySingleton<SayAnythingUi>
    {

        public bool _Debug = true;
        public SayAnythingLogic _LinkToLogic;


        public GameObject _WaitForStartUI;
        public GameObject _QuestioningUI;
        public GameObject _AnsweringUI;
        public GameObject _JudgingAndVotingUI;
        public GameObject _ShowWinnerUI;
        public GameObject _ShowScoreUI;

        public SharedData CurrentData
        {
            get
            {
                if (_Debug)
                {
                    SharedData data = new SharedData();
                    data.judgeUserId = 0;
                    data.judgedAnswerId = 1;
                    data.question = "How are you?";
                    data.state = GameState.ShowScore;
                    data.answers.Add(1, "answer 1");
                    data.answers.Add(2, "answer 2");
                    data.answers.Add(3, "answer 3");
                    data.answers.Add(4, "answer 4");
                    data.answers.Add(5, "answer 5");
                    data.AddVote(1, 2);
                    data.AddVote(1, 3);
                    data.AddVote(2, 1);
                    data.AddVote(2, 1);
                    data.AddVote(3, 1);
                    data.AddVote(3, 1);
                    data.AddVote(4, 1);
                    data.AddVote(4, 1);
                    data.AddVote(5, 1);
                    data.AddVote(5, 1);

                    data.roundScore[0] = 0;
                    data.roundScore[1] = 1;
                    data.roundScore[2] = 2;
                    data.roundScore[3] = 3;
                    data.roundScore[4] = 4;
                    data.roundScore[5] = 5;
                    data.totalScore[0] = 6;
                    data.totalScore[1] = 7;
                    data.totalScore[2] = 8;
                    data.totalScore[3] = 9;
                    data.totalScore[4] = 10;
                    data.totalScore[5] = 11;
                    return data;
                }
                else
                {
                    return _LinkToLogic.Data; ;
                }
            }
        }


        public void FixedUpdate()
        {
            SharedData data = CurrentData;
            if(data != null)
            {
                ShowState(data.state);
            }
        }


        private void ShowState(GameState state)
        {
            var en = SharedData.GetValidStates();

            foreach(GameState s in en)
            {
                GameObject go = GetStateParent(s);
                if (go != null)
                {
                    if (s == state)
                    {
                        if (go.activeSelf == false)
                            go.SetActive(true);
                    }
                    else
                    {
                        if (go.activeSelf == true)
                            go.SetActive(false);
                    }
                }
            }
        }
        public string GetUserName(int id)
        {
            if(Platform.Instance.Controllers.ContainsKey(id))
            {
                return Platform.Instance.Controllers[id].Name;
            }
            return "Someone(" + id + ")";
        }
        public Color GetUserColor(int id)
        {
            return new Color(1, 0.5f, 0.5f, 1);
        }
        private GameObject GetStateParent(GameState state)
        {
            if(state == GameState.WaitForStart)
            {
                return _WaitForStartUI;
            }else if(state == GameState.Questioning)
            {
                return _QuestioningUI;
            }else if(state == GameState.Answering)
            {
                return _AnsweringUI;
            }else if(state == GameState.JudgingAndVoting)
            {
                return _JudgingAndVotingUI;
            }
            else if (state == GameState.ShowWinner)
            {
                return _ShowWinnerUI;
            }
            else if (state == GameState.ShowScore)
            {
                return _ShowScoreUI;
            }
            else
            {
                Debug.LogError("Invalid state requested " + state);
                return null;
            }
        }
    }


}