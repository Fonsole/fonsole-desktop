using UnityEngine;
using System.Collections;
using PPlatform;

public class ControllerList : MonoBehaviour {

    public ControllerUi[] _ControllerUis;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	private void FixedUpdate ()
    {
        int counter = 0;
	    foreach(var v in Platform.Instance.Controllers)
        {
            _ControllerUis[counter].SetVisibility(true);
            _ControllerUis[counter].SetContent(v.Value);
            counter++;
        }

        for(int i = counter; i < _ControllerUis.Length; i++)
        {
            _ControllerUis[i].SetVisibility(false);
        }
	}
}
