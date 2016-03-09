using UnityEngine;
using System.Collections;
namespace PPlatform.SayAnything.Ui
{
	public class AnsweredList : MonoBehaviour
	{
		public AnsweredUi[] _AnsweredUis;

		void OnEnable (){
			for(int i=0; i< _AnsweredUis.Length; i++)
			{
				_AnsweredUis[i].anim.rt.anchoredPosition = new Vector2 (-Screen.width - 500, -Screen.height - 500);
				_AnsweredUis[i].answered = false;
			}
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			int counter = 0;
			foreach(var v in SayAnythingUi.Instance.CurrentData.answers)
			{
				_AnsweredUis[counter].Refresh(v.Key, SayAnythingUi.Instance.CurrentData, counter+1);
				counter++;
			}
			for (int i = counter; i < _AnsweredUis.Length; i++)
			{
				//give them an invalid user. they are going to hide themselves
				_AnsweredUis[counter].Refresh(SharedData.UNDEFINED, SayAnythingUi.Instance.CurrentData);
			}
		}
	}
}