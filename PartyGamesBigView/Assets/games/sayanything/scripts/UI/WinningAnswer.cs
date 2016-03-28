using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PPlatform;

namespace PPlatform.SayAnything.Ui
{
    public class WinningAnswer : UserUi
    {
        public Text _AnswerText;
        public Text _WinnerText;

        public void OnEnable()
        {
            SharedData data = SayAnythingUi.Instance.CurrentData;
            _WinnerText.color = SayAnythingUi.Instance.GetUserColor(data.judgeUserId);
            _AnswerText.text = data.answers[data.judgedAnswerId];
            SetColor(SayAnythingUi.Instance.GetUserColor(data.judgeUserId));
            SetUserName(SayAnythingUi.Instance.GetUserName(data.judgeUserId));
        }
    }
}