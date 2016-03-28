using UnityEngine;
using System.Collections;
using PPlatform;

namespace PPlatform.SayAnything.Ui
{

    /// <summary>
    /// Class for showing a user score, and a number of votes cast in the ccurrent round.
    /// </summary>
    public class ScoreList : MonoBehaviour
    {
        public ScoreUI[] _ScoreUIs;

        void OnEnable()
        {
            int counter = 0;
            foreach (var v in SayAnythingUi.Instance.GetActiveUsersOrderedByConnectionId())
            {
                _ScoreUIs[counter++].gameObject.SetActive(true);
            }

            for (int i = counter; i < _ScoreUIs.Length; ++i)
            {
                _ScoreUIs[i].gameObject.SetActive(false);
            }
        }

        void FixedUpdate()
        {
            int counter = 0;
            foreach (var v in SayAnythingUi.Instance.GetActiveUsersOrderedByConnectionId())
            {
                _ScoreUIs[counter++].Refresh(v, SayAnythingUi.Instance.CurrentData);
            }
        }
    }
}