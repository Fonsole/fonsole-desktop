using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PPlatform;
using Prime31.ZestKit;

namespace PPlatform.SayAnything.Ui
{
    public class WinningAnswer : UserUi
    {
        public Text _AnswerText;
        public Text _WinnerText;
        public RectTransform _WinnerInfo;

        protected override void Awake()
        {
            base.Awake();
            _AnswerText.rectTransform.localScale = Vector3.zero;
            _WinnerInfo.localScale = Vector3.zero;
        }

        public void OnEnable()
        {
            _WinnerText.rectTransform.ZKlocalScaleTo(Vector3.one, 0.5f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.SineOut)
                .start();

            _AnswerText.rectTransform.ZKlocalScaleTo(Vector3.one, 0.5f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.SineOut)
                .setDelay(0.25f)
                .start();

            _WinnerInfo.ZKlocalScaleTo(Vector3.one, 0.5f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.SineOut)
                .setDelay(0.3f)
                .start();

            SharedData data = SayAnythingUi.Instance.CurrentData;
            _WinnerText.color = SayAnythingUi.Instance.GetUserColor(data.judgeUserId);
            _AnswerText.text = data.answers[data.judgedAnswerId];
            SetColor(SayAnythingUi.Instance.GetUserColor(data.judgeUserId));
            SetUserName(SayAnythingUi.Instance.GetUserName(data.judgeUserId));
        }
    }
}