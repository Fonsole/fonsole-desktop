using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PPlatform;
using Prime31.ZestKit;

namespace PPlatform.SayAnything.Ui
{
    public class ScoreUI : UserUi
    {
        public Text _ScoreText;
        public Image _Figurine;
        public Text _RoundScoreText;

        public float ScoreShowTime;
        public float TickTime;
        public float TickScale;

        public void ShowPreviousRoundScore(int userId, SharedData data)
        {
            _RoundScoreText.gameObject.SetActive(false);

            int score = 0;
            data.previousRoundScore.TryGetValue(userId, out score);
            _ScoreText.text = score.ToString();

            SetColor(SayAnythingUi.Instance.GetUserColor(userId));
            SetUserName(SayAnythingUi.Instance.GetUserName(userId));
            _ScoreText.transform.localScale = Vector2.one;
        }

        public void TickUpScore(int userId, SharedData data)
        {
            StartCoroutine(TickScoreCoroutine(userId, data));
        }

        IEnumerator TickScoreCoroutine(int userId, SharedData data)
        {
            int score = 0;
            data.previousRoundScore.TryGetValue(userId, out score);

            int roundScore = 0;
            data.roundScore.TryGetValue(userId, out roundScore);

            if (roundScore != 0)
            {
                _RoundScoreText.gameObject.SetActive(true);
                _RoundScoreText.text = "+" + roundScore;

                Debug.Log(userId + ", Score " + score + ", " + roundScore);

                yield return new WaitForSeconds(ScoreShowTime);

                float scaleDiff = TickScale - 1f;
                for (int i = 0; i < roundScore; ++i)
                {
                    _ScoreText.text = (++score).ToString();
                    _ScoreText.transform.localScale = new Vector3(TickScale, TickScale, TickScale);

                    float time = 0f;
                    while (time < TickTime)
                    {
                        float scale = TickScale - (time / TickTime) * scaleDiff;
                        _ScoreText.transform.localScale = new Vector3(scale, scale, scale);
                        yield return null;

                        time += Time.deltaTime;
                    }
                }

                _RoundScoreText.gameObject.SetActive(false);
            }
        }
    }
}