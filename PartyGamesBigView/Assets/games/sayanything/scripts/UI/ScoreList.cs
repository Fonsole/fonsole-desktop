using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PPlatform;
using Prime31.ZestKit;

namespace PPlatform.SayAnything.Ui
{

    /// <summary>
    /// Class for showing a user score, and a number of votes cast in the ccurrent round.
    /// </summary>
    public class ScoreList : MonoBehaviour
    {
        public ScoreUI[] _ScoreUIs;
        public Text _ScoreText;
        private RectTransform _rt;
        private int _PlayerCount;

        void Awake() {
            _rt = GetComponent<RectTransform>();
            _ScoreText.rectTransform.localScale = Vector3.zero;
            foreach(var v in SayAnythingUi.Instance.GetActiveUsersOrderedByConnectionId())
            {
                _PlayerCount++;
            }
            // can debug fake players here
            //_PlayerCount = 3;
            foreach(ScoreUI ui in _ScoreUIs)
            {
                ui.gameObject.SetActive(false);
            }
        }

        void OnEnable()
        {
            // slide panel down
            _rt.ZKanchoredPositionTo(_rt.anchoredPosition, 0.875f)
                .setFrom(new Vector2(_rt.anchoredPosition.x, _rt.anchoredPosition.y + Screen.height * 3f))
                .setEaseType(EaseType.SineOut)
                .start();
        }

        void OnDisable()
        {
            // reset objects scale / active state
            _ScoreText.rectTransform.localScale = Vector3.zero;
            foreach (ScoreUI ui in _ScoreUIs)
            {
                ui.gameObject.SetActive(false);
            }
        }

        public void ShowScoreInfo()
        {
            // set overall delay on score info
            var showScoreDelay = 0.15f;

            // expand scores title text
            _ScoreText.rectTransform.ZKlocalScaleTo(Vector3.one, 0.5f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.SineOut)
                .setDelay(showScoreDelay)
                .start();

            // expand each player score one after another
            var playerScoreDelay = 0.15f;

            for (int i = 0; i < _PlayerCount; i++)
            {
                _ScoreUIs[i].gameObject.transform.localScale = Vector3.zero;
                _ScoreUIs[i].gameObject.SetActive(true);
                _ScoreUIs[i].gameObject.transform.ZKlocalScaleTo(Vector3.one, 0.25f)
                    .setFrom(Vector3.zero)
                    .setDelay(showScoreDelay + 0.5f + playerScoreDelay)
                    .setEaseType(EaseType.SineOut)
                    .start();
                playerScoreDelay = playerScoreDelay + 0.15f;
            }

            /* old activate / deactivate score uis
            int counter = 0;
            foreach (var v in SayAnythingUi.Instance.GetActiveUsersOrderedByConnectionId())
            {
                _ScoreUIs[counter].gameObject.SetActive(true);
                counter++;
            }
                       
            for (int i = _PlayerCount; i < _ScoreUIs.Length; ++i)
            {
                _ScoreUIs[i].gameObject.SetActive(false);
            }
            */
        }

        void FixedUpdate()
        {
            int counter = 0;
            foreach (var v in SayAnythingUi.Instance.GetActiveUsersOrderedByConnectionId())
            {
                _ScoreUIs[counter].Refresh(v, SayAnythingUi.Instance.CurrentData);
                counter++;
            }
        }
    }
}