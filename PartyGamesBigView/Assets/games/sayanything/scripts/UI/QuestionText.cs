using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace PPlatform.SayAnything.Ui
{

    public class QuestionText : MonoBehaviour
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
        void OnEnable()
        {
            if (_Text == null)
            {
                Debug.Log(gameObject.name + " has missing references. Object will be deactivated ");
                this.gameObject.SetActive(false);
                return;
            }
            if (SayAnythingUi.Instance.CurrentData.question != null)
            {
                _Text.text = SayAnythingUi.Instance.CurrentData.question;
            }
            else
            {

                _Text.text = "No question?";
            }
        }
    }

}
