using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PPlatform.SayAnything.Ui
{
	/// <summary>
	/// AnsweredUI is for the little notification popups that appear when someone has answered
	/// </summary>
	public class DisplayAnswerUi : UserUi
	{

		public Text _Text;
		public GameObject _SelectedMarker;
		public bool displaying = false;
		public bool displayed = false;

		public void Refresh(int userId, SharedData data)
		{
			if (data.answers.ContainsKey(userId))
			{
				//Animate answer popup
				//Debug.Log("Has answer");
				if (displaying == false) {
					displaying = true;
					_Text.text = data.answers[userId];
					SetVisibile(true);

                    //_UsernameText.text = SayAnythingUi.Instance.GetUserName(userId);
                    //_UsernameText.color = SayAnythingUi.Instance.GetUserColor(userId);
                    StartCoroutine(AnswerDisplay (userId));
				}
			}
			else
			{
				//the user of this answer didn't respond -> hide
				//SetVisibile(false);
				//anim.rt.anchoredPosition = new Vector2 (userId * 55 - ((9f*55f)/2f), -Screen.height + 60f);
				//SetColor(new Color(1,1,1,0.5f));
			}
		}


		public IEnumerator AnswerDisplay (int userId){
			anim.StartCoroutine (anim.MovePause (
				new Vector2 (Screen.width + 200, 0),
				new Vector2(0, 0),
				new Vector2 (-Screen.width -1000, 0), 5f, 3f));

			yield return new WaitForSeconds (5f);
			displayed = true;
		}
	}
}