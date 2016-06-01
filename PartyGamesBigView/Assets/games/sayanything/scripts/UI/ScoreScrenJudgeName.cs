using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Prime31.ZestKit;

namespace PPlatform.SayAnything.Ui
{
    public class ScoreScrenJudgeName : MonoBehaviour
    {
        private RectTransform _rt;
        public Text JudgeText;
        public Image Figurine;

        protected void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _rt.localScale = Vector3.zero;
        }

        public void OnEnable()
        {
            /*_rt.ZKanchoredPositionTo(_rt.anchoredPosition, 0.875f)
                .setFrom(new Vector2(_rt.anchoredPosition.x, _rt.anchoredPosition.y + 300f))
                .setEaseType(EaseType.SineOut)
                .start();*/

            _rt.ZKlocalScaleTo(Vector3.one, 0.5f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.SineOut)
                .setDelay(0.5f)
                .start();

            SharedData data = SayAnythingUi.Instance.CurrentData;
            Figurine.color = SayAnythingUi.Instance.GetUserColor(data.judgeUserId);
            JudgeText.text = SayAnythingUi.Instance.GetUserName(data.judgeUserId);
        }

        void OnDisable()
        {
            _rt.localScale = Vector3.zero;
        }
    }
}