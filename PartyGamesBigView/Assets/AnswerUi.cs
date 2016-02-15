using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PPlatform.SayAnything.UI
{
    public class AnswerUi : MonoBehaviour
    {
        /// <summary>
        /// Textfield. Shows either the answer or the score
        /// </summary>
        public Text _Text;



        public GameObject _UsernameParent;
        public Text _UsernameText;
        public Image[] _UsernameColors;


        public GameObject _VoteParent;

        public void Refresh(int userId, SharedData data)
        {
            if(data.answers.ContainsKey(userId))
            {
                if (data.state == GameState.JudgingAndVoting && data.judgeUserId == -1)
                {
                    //judge didnt decide yet. only show answers
                    _Text.text = data.answers[userId];
                    SetColor(Color.white);
                    _UsernameParent.SetActive(false);
                    _VoteParent.SetActive(false);
                }else if (data.state == GameState.JudgingAndVoting && data.judgeUserId != -1)
                {
                    _Text.text = data.answers[userId];
                    SetColor(Color.white);
                    _VoteParent.SetActive(true);
                    _UsernameParent.SetActive(false);
                }
                else if (data.state == GameState.ShowWinner)
                {
                    _Text.text = data.answers[userId];
                    SetColor(SayAnythingUi.Instance.GetUserColor(userId));
                    _VoteParent.SetActive(true);
                    _UsernameParent.SetActive(true);
                    _UsernameText.text = SayAnythingUi.Instance.GetUserName(userId);
                }
                else if (data.state == GameState.ShowScore)
                {
                    _VoteParent.SetActive(true);
                    _UsernameParent.SetActive(true);
                }

                SetVisibile(true);
            }
            else
            {
                //the user of this answer didn't respond -> hide
                SetVisibile(false);
            }
        }

        private void SetColor(Color c)
        {
            if (_UsernameColors == null)
                return;

            for(int i = 0; i < _UsernameColors.Length; i++)
            {
                _UsernameColors[i].color = c;
            }
        }

        private void SetVisibile(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }


    }
}