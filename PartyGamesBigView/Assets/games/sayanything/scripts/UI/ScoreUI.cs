using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PPlatform;

namespace PPlatform.SayAnything.Ui
{
    public class ScoreUI : UserUi
    {
        public Text _ScoreText;
        public Image _Figurine;

        public void Refresh(int userId, SharedData data)
        {
            if (data.roundScore.ContainsKey(userId))
            {
                _ScoreText.text = data.roundScore[userId].ToString();

                SetColor(SayAnythingUi.Instance.GetUserColor(userId));
                SetUserName(SayAnythingUi.Instance.GetUserName(userId));
            }
        }
    }
}