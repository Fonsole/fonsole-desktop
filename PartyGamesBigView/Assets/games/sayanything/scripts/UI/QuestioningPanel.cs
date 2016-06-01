using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Prime31.ZestKit;

namespace PPlatform.SayAnything.Ui
{
    public class QuestioningPanel : MonoBehaviour, ComponentTweener
    {
        public RectTransform QuestionPanel;

        public float TweenInTime;
        public float TweenOutTime;

        void OnEnable()
        {
            Vector2 originalPosition = QuestionPanel.transform.localPosition;
            originalPosition.x = 0f;

            if (TweenInTime != 0f)
            {
                float direction = 1f;
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                    direction = -1f;
                QuestionPanel.transform.localPosition = originalPosition + (Vector2.right * direction * Screen.width * 1.5f);
                //tween in
                QuestionPanel.ZKlocalPositionTo(originalPosition, TweenInTime)
                    .setEaseType(EaseType.SineOut)
                    .start();
            }
            else
            {
                QuestionPanel.transform.localPosition = originalPosition;
            }
        }

        public void TweenOut(float time)
        {
            Vector2 targetPosition = QuestionPanel.transform.localPosition;
            targetPosition.x = 0;

            float direction = 1f;
            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                direction = -1f;
            targetPosition -= (Vector2.right * direction * Screen.width * 1.5f);
            QuestionPanel.ZKlocalPositionTo(targetPosition, TweenOutTime)
                .setDelay(Mathf.Max(0f, time - TweenOutTime))
                .start();
        }
    }
}
