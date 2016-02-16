using UnityEngine;
using System.Collections;
using PPlatform;

namespace PPlatform.SayAnything.UI
{
    public class ControllerList : MonoBehaviour
    {

        public ControllerUi[] _ControllerUis;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            int counter = 0;
            foreach (var v in SayAnythingUi.Instance.GetActiveUsers())
            {
                _ControllerUis[counter].Refresh(v, SayAnythingUi.Instance.CurrentData);
                counter++;
            }
            for (int i = counter; i < _ControllerUis.Length; i++)
            {
                //give them an invalid user. they are going to hide themselves
                _ControllerUis[i].Refresh(SharedData.UNDEFINED, SayAnythingUi.Instance.CurrentData);
            }
        }
    }
}
