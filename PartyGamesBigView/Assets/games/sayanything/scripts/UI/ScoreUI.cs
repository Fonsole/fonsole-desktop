using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PPlatform;

namespace PPlatform.SayAnything.Ui
{
    public class ScoreUI : UserUi
    {
        public Text _ScoreText;

        public Image _OneVoteIcon;
        public Image _TwoVoteIcon;
        public Image _OneVoteFigurine;
        public Image _TwoVoteFigurine;
        public Image _Figurine;

        void OnEnable()
        {
            _Figurine.gameObject.SetActive(false);

            _OneVoteIcon.gameObject.SetActive(false);
            _OneVoteFigurine.gameObject.SetActive(false);

            _TwoVoteIcon.gameObject.SetActive(false);
            _TwoVoteFigurine.gameObject.SetActive(false);
       }
        
        public void Refresh(int userId, SharedData data)
        {
            if (data.roundScore.ContainsKey(userId))
            {
                _ScoreText.text = data.roundScore[userId].ToString();

                if (data.votes.ContainsKey(userId))
                {
                    if (data.votes[userId].Count == 1)
                    {
                        _OneVoteIcon.gameObject.SetActive(true);
                        _OneVoteFigurine.gameObject.SetActive(true);
                    }
                    else if (data.votes[userId].Count == 2)
                    {
                        _TwoVoteIcon.gameObject.SetActive(true);
                        _TwoVoteFigurine.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _Figurine.gameObject.SetActive(true);
                }

                SetColor(SayAnythingUi.Instance.GetUserColor(userId));
                SetUserName(SayAnythingUi.Instance.GetUserName(userId));
            }
        }
    }
}