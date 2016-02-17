﻿using UnityEngine;
using System.Collections;
using PPlatform.SayAnything;
using System;
using System.Linq;
using PPlatform.Helper;
using System.Collections.Generic;

namespace PPlatform.SayAnything.Ui
{
    /// <summary>
    /// Signeton while the say anything game is running.
    /// 
    /// All UI elements get their shared data + platform data via this class.
    /// 
    /// This allows to feed in mocking data to test and design the ui without
    /// having to run the whole game.
    /// </summary>
    public class SayAnythingUi : UnitySingleton<SayAnythingUi>
    {

        public bool _Debug = true;
        public SayAnythingLogic _LinkToLogic;


        public GameObject _WaitForStartUI;
        public GameObject _QuestioningUI;
        public GameObject _AnsweringUI;
        public GameObject _JudgingAndVotingUI;


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

        private void Start()
        {

        }

        //private void TestColors()
        //{

        //    for (int i = 0; i < 100; i++)
        //    {
        //        Color c = UnityEngine.Random.ColorHSV(0, 1, 0.5f, 0.75f, 0.4f, 0.9f, 1, 1);
        //        Debug.Log("<color=" + "#" + ColorToHex(c) + ">" + c + "</color>");
        //    }
        //}
        //string ColorToHex(Color32 color)
        //{
        //    string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        //    return hex;
        //}
 

        public int[] GetActiveUsers()
        {
            return Platform.Instance.Controllers.Keys.ToArray();
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

            if (state == GameState.WaitForStart)
            {
                _WaitForStartUI.SetActive(true);
                _QuestioningUI.SetActive(false);
                _AnsweringUI.SetActive(false);
                _JudgingAndVotingUI.SetActive(false);
            }
            else if (state == GameState.Questioning)
            {
                _WaitForStartUI.SetActive(false);
                _QuestioningUI.SetActive(true);
                _AnsweringUI.SetActive(false);
                _JudgingAndVotingUI.SetActive(false);
            }
            else if (state == GameState.Answering)
            {
                _WaitForStartUI.SetActive(false);
                _QuestioningUI.SetActive(false);
                _AnsweringUI.SetActive(true);
                _JudgingAndVotingUI.SetActive(false);
            }
            else if (state == GameState.JudgingAndVoting)
            {
                _WaitForStartUI.SetActive(false);
                _QuestioningUI.SetActive(false);
                _AnsweringUI.SetActive(false);
                _JudgingAndVotingUI.SetActive(true);
            }
            else if (state == GameState.ShowWinner)
            {
                _WaitForStartUI.SetActive(false);
                _QuestioningUI.SetActive(false);
                _AnsweringUI.SetActive(false);
                _JudgingAndVotingUI.SetActive(true);
            }
            else if (state == GameState.ShowScore)
            {
                _WaitForStartUI.SetActive(false);
                _QuestioningUI.SetActive(false);
                _AnsweringUI.SetActive(false);
                _JudgingAndVotingUI.SetActive(true);
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




        public Color[] mColorList = new Color[]
        {
            ToColor(170, 114, 57),
            ToColor(170, 136, 57),
            ToColor(48, 61, 116),
            ToColor(38, 91, 106),

            ToColor(41, 150, 41),
            ToColor(95, 39, 126),
            ToColor(188, 188, 51),
            ToColor(188, 51, 51),

            ToColor(3, 36, 75),
            ToColor(114, 34, 0),
            ToColor(0, 78, 47),
            ToColor(114, 70, 0)
        };

        public Dictionary<int, Color> mUserColor = new Dictionary<int, Color>();

        ///// <summary>
        ///// If we have a new user who needs a new color we check first if there is a color given to a user that doesn't exist anymore.
        ///// After that we give out one of the free colors
        ///// </summary>
        //private void RefreshColorDictionary()
        //{
        //    Dictionary<int, Color> newDict = new Dictionary<int, Color>();

        //    foreach(var v in mUserColor)
        //    {
        //        if(IsUserAvailable(v.Key) == false)
        //        {
        //            newDict.Add(v.Key, v.Value);
        //        }
        //    }
        //    mUserColor = newDict;
        //}

        public bool IsUserAvailable(int userId)
        {
            if (Platform.Instance.Controllers.ContainsKey(userId))
                return true;
            return false;
        }

        public void AllocNewColor(int userId)
        {
            //TODO: use defined colors first before using random ones.
            mUserColor[userId] = UnityEngine.Random.ColorHSV(0, 1, 0.5f, 0.75f, 0.4f, 0.9f, 1, 1);
        }
        public Color GetUserColor(int id)
        {
            if(IsUserAvailable(id) == false)
            {
                //user that are offline get a grey color
                return new Color(0.25f, 0.25f, 0.25f, 1);
            }
            else
            {
                if (mUserColor.ContainsKey(id) == false)
                {
                    AllocNewColor(id);
                }
                return mUserColor[id];
            }
            
        }


        public static Color ToColor(int r, int g, int b)
        {
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, 1);
        }

    }


}