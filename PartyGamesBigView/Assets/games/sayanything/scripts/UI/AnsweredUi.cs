using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prime31.ZestKit;

namespace PPlatform.SayAnything.Ui
{
	/// <summary>
	/// AnsweredUI is for the little notification popups that appear when someone has answered
	/// </summary>
	public class AnsweredUi : UserUi, ComponentTweener
	{

		public GameObject _SelectedMarker;
		public bool answered = false;
        RectTransform _rt;

        Vector3 TargetScale;

        protected override void Awake()
        {
            base.Awake();
            _rt = GetComponent<RectTransform>();
            TargetScale = _rt.localScale;
        }

        public void OnEnable()
        {
            _rt.localScale = Vector3.zero;
        }

		public void Refresh(int userId, SharedData data, int order = 1)
		{
			if (data.answers.ContainsKey(userId))
			{
                //Animate answer popup
                if (!answered) {
					answered = true;
					SetVisibile(true);
					SetColor(Color.Lerp(SayAnythingUi.Instance.GetUserColor(userId), Color.white, 0.575f));
					AnswerPopup ((order%2==0)? order/2: -order/2);
				}
			}
			else
			{
				//the user of this answer didn't respond -> hide
				SetVisibile(false);
				//anim.rt.anchoredPosition = new Vector2 (userId * 55 - ((9f*55f)/2f), -Screen.height + 60f);
				//SetColor(new Color(1,1,1,0.5f));
			}
		}


		public void AnswerPopup (int position) {
            AudioManager.Instance.OnAnswerSubmitted();

            anim.StartCoroutine (anim.Move (
				new Vector2 (position *150f, -460f),
				new Vector2 (position *150f, -460f),
				new Vector2 (position *150f, -460f), 1f)); 

            _rt.ZKlocalScaleTo(TargetScale, 0.25f)
                .setDelay(0.15f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.QuadInOut)
                .start();
        }

        public void TweenOut(float time)
        {
            _rt.localScale = TargetScale;
            _rt.ZKlocalScaleTo(Vector3.zero, 0.35f)
                .setFrom(TargetScale)
                .setDelay(time - 0.45f)
                .start();
        }
	}
}