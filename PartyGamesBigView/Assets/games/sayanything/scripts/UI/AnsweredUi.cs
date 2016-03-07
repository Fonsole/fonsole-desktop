using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PPlatform.SayAnything.Ui
{
	/// <summary>
	/// AnsweredUI is for the little notification popups that appear when someone has answered
	/// </summary>
	public class AnsweredUi : UserUi
	{

		public GameObject _SelectedMarker;
		public bool answered = false;

		public void Refresh(int userId, SharedData data)
		{
			if (data.answers.ContainsKey(userId))
			{
                //Animate answer popup
                if (!answered) {
					answered = true;
					SetVisibile(true);
					SetColor(SayAnythingUi.Instance.GetUserColor(userId));
					AnswerPopup (userId);
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


		public void AnswerPopup (int userId) {
            anim.StartCoroutine (anim.Move (
				new Vector2 (userId * 55 - ((9f*55f)/2f), -500f),
				new Vector2 (userId * 55 - ((9f*55f)/2f), -425f),
				new Vector2 (userId * 55 - ((9f*55f)/2f), -460f), 5f));
		}
	}
}