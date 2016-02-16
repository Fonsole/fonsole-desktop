using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PPlatform;
using PPlatform.SayAnything.Ui;

/// <summary>
/// Allows to fill a given text element automatically with a judge name.
/// 
/// Keeps checking if the judge changes and refreshes if there is a change.
/// </summary>
public class JudgeName : MonoBehaviour {


    public string _TextBeforeName = "";
    public Text _Name;
    public string _TextAfterName = "";
    private int lastId = -1;

	void FixedUpdate ()
    {
        if (SayAnythingUi.Instance != null && SayAnythingUi.Instance.CurrentData != null)
        {
            int id = SayAnythingUi.Instance.CurrentData.judgeUserId;
            if (lastId != id)
            {
                this._Name.text = _TextBeforeName + SayAnythingUi.Instance.GetUserName(id) + _TextAfterName;
                lastId = id;
            }
        }
	}


    private void OnEnable()
    {
    }
}
