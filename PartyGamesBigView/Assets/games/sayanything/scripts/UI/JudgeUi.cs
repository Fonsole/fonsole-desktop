using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prime31.ZestKit;
using System;

namespace PPlatform.SayAnything.Ui
{
    public class JudgeUi : MonoBehaviour, ComponentTweener
    {
        // "Q:" text
        public Text _Label;
        // Question text
        public Text _Text;
        private RectTransform _rt;

        void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            /*_rt.ZKanchoredPositionTo(_rt.anchoredPosition, 0.875f)
                .setFrom(new Vector2(_rt.anchoredPosition.x, _rt.anchoredPosition.y + 300f))
                .setEaseType(EaseType.SineOut)
                .start();*/

            if (_Label != null)
            {
                _Label.rectTransform.localScale = Vector3.zero;
                _Label.rectTransform.ZKlocalScaleTo(Vector3.one, 0.25f)
                    .setFrom(Vector3.zero)
                    .setDelay(0.5f)
                    .setEaseType(EaseType.SineOut)
                    .start();
            }

            _Text.rectTransform.localScale = Vector3.zero;
            _Text.rectTransform.ZKlocalScaleTo(Vector2.one, 0.5f)
                .setFrom(Vector3.zero)
                .setDelay(0.5f)
                .setEaseType(EaseType.SineOut)
                .start();

            AudioManager.Instance.PlayWhoosh(0.5f);
        }

        void FixedUpdate()
        {
            SharedData data = SayAnythingUi.Instance.CurrentData;

            if (SayAnythingUi.Instance.CurrentData.question != null)
            {
                _Text.text = SayAnythingUi.Instance.CurrentData.question;
            }
            else
            {
                _Text.text = "No question?";
            }
        }

        public void TweenOut(float time)
        {
            if (_Label != null)
            {
                _Label.rectTransform.ZKlocalScaleTo(Vector3.zero, 0.25f)
                    .setFrom(Vector3.zero)
                    .setDelay(time-0.25f)
                    .setEaseType(EaseType.SineOut)
                    .start();
            }

            _Text.rectTransform.ZKlocalScaleTo(Vector2.zero, 0.35f)
                .setFrom(Vector3.one)
                .setDelay(time-0.35f)
                .setEaseType(EaseType.SineOut)
                .start();
        }
    }
}
