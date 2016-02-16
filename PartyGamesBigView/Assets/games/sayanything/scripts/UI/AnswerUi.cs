using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PPlatform.SayAnything.Ui
{
    /// <summary>
    /// AnswerUI shows user information based on a UserID and the current state of the game.
    /// 
    /// It ignores the current controllers online and only works with the games data to allow
    /// answers to still be shown even if a user disconnects / reconnects.
    /// 
    /// AnswerUI has to be always part of an AnswerList! The answer list makes the decision when the
    /// answer UI's have to be refreshed and gives it a user id in the process. This user id is used
    /// to find out all data shown.
    /// </summary>
    public class AnswerUi : UserUi
    {

        /// <summary>
        /// Textfield. Shows either the answer or the score or nothing
        /// </summary>
        public Text _Text;
        public GameObject _SelectedMarker;
        public VoteList _VoteParent;



        private void Awake()
        {

        }



        public void Refresh(int userId, SharedData data)
        {
            if (data.answers.ContainsKey(userId))
            {
                if (data.state == GameState.JudgingAndVoting && data.judgedAnswerId == -1)
                {
                    //judge didnt decide yet. only show answers
                    _Text.text = data.answers[userId];
                    SetColor(Color.white);
                    _UsernameParent.SetActive(false);
                    _VoteParent.gameObject.SetActive(false);
                    _SelectedMarker.SetActive(false);
                }
                else if (data.state == GameState.JudgingAndVoting && data.judgedAnswerId != -1)
                {
                    _Text.text = data.answers[userId];
                    SetColor(Color.white);
                    _VoteParent.gameObject.SetActive(true);
                    _VoteParent.Refresh(userId, data);
                    _UsernameParent.SetActive(false);
                    _SelectedMarker.SetActive(false);
                }
                else if (data.state == GameState.ShowWinner)
                {
                    _Text.text = data.answers[userId];
                    SetColor(SayAnythingUi.Instance.GetUserColor(userId));
                    _VoteParent.gameObject.SetActive(true);
                    _VoteParent.Refresh(userId, data);
                    _UsernameParent.SetActive(true);
                    _UsernameText.text = SayAnythingUi.Instance.GetUserName(userId);

                    if (userId == data.judgedAnswerId)
                    {
                        _SelectedMarker.SetActive(true);
                    }
                    else
                    {

                        _SelectedMarker.SetActive(false);
                    }
                }
                else if (data.state == GameState.ShowScore)
                {
                    int score = 0;
                    int totalScore = 0;

                    data.roundScore.TryGetValue(userId, out score);
                    data.totalScore.TryGetValue(userId, out totalScore);
                    _Text.text = "Score: " + score + "\nTotal Score: " + totalScore;
                    SetColor(SayAnythingUi.Instance.GetUserColor(userId));
                    _VoteParent.gameObject.SetActive(true);
                    _VoteParent.Refresh(userId, data);
                    _UsernameParent.SetActive(true);
                    _UsernameText.text = SayAnythingUi.Instance.GetUserName(userId);
                    if (userId == data.judgedAnswerId)
                    {
                        _SelectedMarker.SetActive(true);
                    }
                    else
                    {

                        _SelectedMarker.SetActive(false);
                    }
                }

                SetVisibile(true);
            }
            else
            {
                //the user of this answer didn't respond -> hide
                SetVisibile(false);
            }
        }

    }
}