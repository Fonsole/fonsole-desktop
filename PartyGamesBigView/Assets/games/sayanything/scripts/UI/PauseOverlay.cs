using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PPlatform.SayAnything.Ui
{
    public class PauseOverlay : MonoBehaviour
    {
        public Text PausedPlayerText;
        public string PausedPlayerString;
        bool _switched = false;

        void Awake()
        {
            PausedPlayerString = PausedPlayerText.text;
        }

        void OnEnable()
        {
            SharedData data = SayAnythingUi.Instance.CurrentData;

            PausedPlayerText.text = string.Format(PausedPlayerString, SayAnythingUi.Instance.GetUserName(data.pausePlayerId));

            _switched = false;
        }

        void Update()
        {
            SharedData data = SayAnythingUi.Instance.CurrentData;

            if (!_switched && data.pauseTime > SharedData.PauseCutoff)
            {
                PausedPlayerText.text = string.Format(PausedPlayerString, "Anyone");
                _switched = true;
            }
        }
    }
}