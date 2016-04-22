using UnityEngine;
using System.Collections;

namespace PPlatform.SayAnything.Ui
{
    /// <summary>
    /// Handles the transition for presenting the question, the winning answer, and showing scores.
    /// </summary>
    public class WinnerUi : MonoBehaviour {

        public GameObject _QuestionPanel; // JudgeUI.cs
        public GameObject _WinnerPanel; // WinningAnswer.cs
        public GameObject _ScorePanel; // ScoreList.cs

        private bool _HasShownScores;

        void OnEnable()
        {
            // Activate panels
            _QuestionPanel.SetActive(true);
            _WinnerPanel.SetActive(true);
            _ScorePanel.SetActive(true);
        }

        void OnDisable()
        {
            _HasShownScores = false;
        }

        void FixedUpdate()
        {            
            // Show Score Panel info on state change
            SharedData data = SayAnythingUi.Instance.CurrentData;
            if (data.state == GameState.ShowScore && !_HasShownScores)
            {
                // old activate panel
                // if (!_ScorePanel.activeSelf)
                //    _ScorePanel.SetActive(true);
                   
                // tell scorepanel to show info
                _ScorePanel.GetComponent<ScoreList>().ShowScoreInfo();
                _HasShownScores = true;          
            }
        }
    }
}
