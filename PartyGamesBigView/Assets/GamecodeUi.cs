using UnityEngine;
using System.Collections;
using PPlatform;
using UnityEngine.UI;

namespace PPlatform.Ui
{
    public class GamecodeUi : MonoBehaviour
    {
        public Text _Text;
        public InputField _InputField;

        private string mLastKnownCode = "";

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if (mLastKnownCode != Platform.Instance.GameCode)
            {
                mLastKnownCode = Platform.Instance.GameCode;
                if (_Text != null)
                    _Text.text = mLastKnownCode;
                if (_InputField != null)
                    _InputField.text = mLastKnownCode;
            }
        }
    }

}
