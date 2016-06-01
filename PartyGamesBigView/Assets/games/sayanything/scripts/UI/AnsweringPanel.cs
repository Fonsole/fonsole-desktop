using UnityEngine;
using System.Collections;
using Prime31.ZestKit;

namespace PPlatform.SayAnything.Ui
{
    public class AnsweringPanel : MonoBehaviour, ComponentTweener
    {
        public RectTransform AnswerPanel;
        public GameObject TimerUI;
        Vector3 TargetScale;

        void Awake()
        {
            TargetScale = AnswerPanel.localScale;
        }

        void OnEnable()
        {
            AnswerPanel.localScale = Vector3.zero;
            AnswerPanel.ZKlocalScaleTo(TargetScale, 0.35f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.QuadInOut)
                .start();

            if (TimerUI != null)
                TimerUI.SetActive(true);
        }

        public void TweenOut(float time)
        {
            AnswerPanel.localScale = TargetScale;
            AnswerPanel.ZKlocalScaleTo(Vector3.zero, 0.35f)
                .setFrom(TargetScale)
                .setDelay(time - 0.45f)
                .start();

            if (TimerUI != null)
                TimerUI.SetActive(false);
        }
    }
}
