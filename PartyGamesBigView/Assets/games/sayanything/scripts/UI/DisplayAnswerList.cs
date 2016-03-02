using UnityEngine;
using System.Collections;
namespace PPlatform.SayAnything.Ui
{
	public class DisplayAnswerList : MonoBehaviour
	{
		public DisplayAnswerUi[] _DisplayAnswerUis;

		void OnEnable (){
			for(int i=0; i< _DisplayAnswerUis.Length; i++)
			{
				_DisplayAnswerUis[i].anim.rt.anchoredPosition = new Vector2 (-Screen.width - 600, -Screen.height - 600);
				_DisplayAnswerUis[i].displaying = false;
				_DisplayAnswerUis[i].displayed = false;
			}
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			int counter = 0;
			foreach(var v in SayAnythingUi.Instance.CurrentData.answers)
			{
				_DisplayAnswerUis[counter].Refresh(v.Key, SayAnythingUi.Instance.CurrentData);
				if (_DisplayAnswerUis [counter].displayed) {
					counter++;
				}
			}
			for (int i = counter; i < _DisplayAnswerUis.Length; i++)
			{
				//give them an invalid user. they are going to hide themselves
				_DisplayAnswerUis[i].Refresh(SharedData.UNDEFINED, SayAnythingUi.Instance.CurrentData);
			}
		}
	}
}