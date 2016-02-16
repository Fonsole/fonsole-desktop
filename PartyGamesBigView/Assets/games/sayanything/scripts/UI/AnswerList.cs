using UnityEngine;
using System.Collections;
namespace PPlatform.SayAnything.Ui
{
    public class AnswerList : MonoBehaviour
    {
        public AnswerUi[] _AnswerUis;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            int counter = 0;
            foreach(var v in SayAnythingUi.Instance.CurrentData.answers)
            {
                _AnswerUis[counter].Refresh(v.Key, SayAnythingUi.Instance.CurrentData);
                counter++;
            }
            for (int i = counter; i < _AnswerUis.Length; i++)
            {
                //give them an invalid user. they are going to hide themselves
                _AnswerUis[i].Refresh(SharedData.UNDEFINED, SayAnythingUi.Instance.CurrentData);
            }
        }
    }
}