using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace PPlatform.SayAnything.Ui
{
	public class DisplayAnswerList : MonoBehaviour
	{
        public DisplayAnswerUi[] _DisplayAnswerUis;
        public KeyValuePair<int, string>[] ShuffledAnswers;

		void OnEnable (){
			for(int i=0; i< _DisplayAnswerUis.Length; i++)
			{
                // answerlist has no animator components
                if (_DisplayAnswerUis[i].anim)
                {
				    _DisplayAnswerUis[i].anim.rt.anchoredPosition = new Vector2 (-Screen.width - 600, -Screen.height - 600);             
                }
				_DisplayAnswerUis[i].displaying = false;
				_DisplayAnswerUis[i].displayed = false;
			}
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			int counter = 0;
            if (ShuffledAnswers == null)
                GetShuffledAnswers();
            
			foreach(var v in ShuffledAnswers)
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

        void GetShuffledAnswers()
        {
            ShuffledAnswers = new KeyValuePair<int, string>[SayAnythingUi.Instance.CurrentData.answers.Keys.Count];
            int counter = 0;
            foreach (KeyValuePair<int, string> answer in SayAnythingUi.Instance.CurrentData.answers)
                ShuffledAnswers[counter++] = answer;

            ArrayHelper.Shuffle<KeyValuePair<int, string>>(ShuffledAnswers);
        }

        void OnDisable()
        {
            ShuffledAnswers = null;
        }
	}
}