using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PPlatform;
using PPlatform.SayAnything.UI;

public class JudgeName : MonoBehaviour {


    public Text _Name;

    public string _TextAfterName;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
	    
	}


    private void OnEnable()
    {
        if(SayAnythingUi.Instance != null && SayAnythingUi.Instance.CurrentData != null)
        {
            int id = SayAnythingUi.Instance.CurrentData.judgedAnswerId;

            if(Platform.Instance.Controllers.ContainsKey(id))
            {
                Controller c = Platform.Instance.Controllers[id];
                this._Name.text = c.Name;
            }
            else
            {
                this._Name.text = "Someone";
            }
        }
    }
}
