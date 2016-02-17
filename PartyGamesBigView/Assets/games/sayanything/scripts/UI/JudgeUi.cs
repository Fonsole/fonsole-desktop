using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace PPlatform.SayAnything.Ui
{
    public class JudgeUi : MonoBehaviour
    {


        public Text _Text;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        void FixedUpdate()
        {
            SharedData data = SayAnythingUi.Instance.CurrentData;

            if(data.state != GameState.ShowScore)
            {
                if (SayAnythingUi.Instance.CurrentData.question != null)
                {
                    _Text.text = SayAnythingUi.Instance.CurrentData.question;
                }
                else
                {

                    _Text.text = "No question?";
                }
            }else
            {
                //Show the judges score
                _Text.text = SayAnythingUi.Instance.GetUserScoreText(data.judgeUserId);
            }

        }
    }
}
