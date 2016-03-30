using UnityEngine;
using System.Collections;

namespace PPlatform.SayAnything.Ui
{
    /// <summary>
    /// Handles the transition for presenting the question, the winning answer, and showing scores.
    /// </summary>
    public class WinnerUi : MonoBehaviour {

        public GameObject _QuestionPanel;
        public GameObject _WinnerPanel;
        public GameObject _ScorePanel;

        void OnEnable()
        {
            // Immediately show question panel
            _QuestionPanel.SetActive(true);
            // Wait then show winning answer panel
            StartCoroutine(ShowWinnerPanel());
        }

        void FixedUpdate()
        {
            // Show Score Panel on state change
            SharedData data = SayAnythingUi.Instance.CurrentData;
            if (data.state == GameState.ShowScore)
            {
                if (!_ScorePanel.activeSelf)
                    _ScorePanel.SetActive(true);
            }
        }

        IEnumerator ShowWinnerPanel()
        {
            yield return new WaitForSeconds(1f);
            _WinnerPanel.SetActive(true);
        }
    }
}
