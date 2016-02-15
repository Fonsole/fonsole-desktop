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
                    data.judgedAnswerId = 1;
                    data.state = GameState.JudgingAndVoting;
                    data.answers.Add(1, "answer 1");
                    data.answers.Add(2, "answer 2");
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
                //ShowState(data.state);
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
            return "Someone";
        }
        public Color GetUserColor(int id)
        {
            return Color.red;
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