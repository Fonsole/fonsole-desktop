using UnityEngine;
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
    public class SayAnythingUi : SceneSingleton<SayAnythingUi>
    {

        public bool _Debug = true;
        public SayAnythingLogic _LinkToLogic;


        public GameObject _WaitForStartUI;
        public GameObject _QuestioningUI;
        public GameObject _AnsweringUI;
        public GameObject _JudgingAndVotingUI;

        public Color[] _PlayerColors = new Color[]
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

        private Dictionary<int, Color> mUserColor = new Dictionary<int, Color>();

        //to help checking for preset colors that are already used
        private Dictionary<Color, int> mColorUser = new Dictionary<Color, int>();

        public SharedData CurrentData
        {
            get
            {
                if (_Debug)
                {
                    return GetDebugData();
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
        private SharedData GetDebugData()
        {

            SharedData data = new SharedData();
            data.judgeUserId = 0;
            data.judgedAnswerId = 1;
            data.question = "Which two people (real or fictional) would you most like to see fight each other?";
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

        /// <summary>
        /// Creats a string showing a users round and total score.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserScoreText(int userId)
        {
            int score = 0;
            int totalScore = 0;
            SharedData data = CurrentData;
            data.roundScore.TryGetValue(userId, out score);
            data.totalScore.TryGetValue(userId, out totalScore);
            return "Score: " + score + "\nTotal Score: " + totalScore;
        }

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





        /// <summary>
        /// If we have a new user who needs a new color we check first if there is a color given to a user that doesn't exist anymore.
        /// After that we give out one of the free colors
        /// </summary>
        private void RefreshColorDictionary()
        {
            Dictionary<int, Color> newDict = new Dictionary<int, Color>();
            Dictionary<Color, int> newColorDict = new Dictionary<Color, int>();
            foreach (var v in mUserColor)
            {
                //keep the color in the list if it is an active user
                if (IsUserAvailable(v.Key))
                {
                    newDict.Add(v.Key, v.Value);
                    newColorDict.Add(v.Value, v.Key);
                }
            }
            mUserColor = newDict;
            mColorUser = newColorDict;
        }

        public bool IsUserAvailable(int userId)
        {
            if (Platform.Instance.Controllers.ContainsKey(userId))
                return true;
            return false;
        }

        public void AllocNewColor(int userId)
        {
            //first throw out all colors that aren't used by known users
            RefreshColorDictionary();

            bool found = false;
            Color result = Color.white;

            //look for the next free color
            for (int i = 0; i < _PlayerColors.Length; i++ )
            {
                if(mColorUser.ContainsKey(_PlayerColors[i]) == false)
                {
                    //found a unused one
                    result = _PlayerColors[i];
                    found = true;
                    break;
                }
            }


            //backup strategy. It is unclear how many users we need to store (as some might be offline for some time and then
            //return?)
            //in case there are more users than colors and all are used -> create a random one
            if (found == false)
            {
                result = UnityEngine.Random.ColorHSV(0, 1, 0.5f, 0.75f, 0.4f, 0.9f, 1, 1);
            }

            mUserColor[userId] = result;
            mColorUser.Add(result, userId);
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