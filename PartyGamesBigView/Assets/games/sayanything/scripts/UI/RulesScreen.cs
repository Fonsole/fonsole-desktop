using UnityEngine;
using System.Collections;
using System;
using Prime31.ZestKit;

namespace PPlatform.SayAnything.Ui
{
    public class RulesScreen : MonoBehaviour, ComponentTweener
    {
        public RectTransform RulesLabel;
        public RectTransform LabelsBG;

        void OnEnable()
        {
            LabelsBG.localScale = Vector3.zero;
            LabelsBG.ZKlocalScaleTo(Vector3.one, 0.35f)
                .setFrom(Vector3.zero)
                .setDelay(0.2f)
                .setEaseType(EaseType.SineOut)
                .start();

            RulesLabel.localScale = Vector3.zero;
            RulesLabel.ZKlocalScaleTo(Vector3.one, 0.5f)
                .setFrom(Vector3.zero)
                .setDelay(0.55f)
                .setEaseType(EaseType.SineOut)
                .start();
        }

        public void TweenOut(float time)
        {
            LabelsBG.localScale = Vector3.one;
            LabelsBG.ZKlocalScaleTo(Vector3.zero, 0.35f)
                .setFrom(Vector3.one)
                .setDelay(time-0.45f)
                .start();
        }
    }
}
