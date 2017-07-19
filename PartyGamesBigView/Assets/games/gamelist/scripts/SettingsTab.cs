using DebugTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsTab : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void OnConsoleToggle(bool b)
    {
        DebugConsole.sShowConsole = b;
    }

    public void OnDebugUIToggle(bool b)
    {
        DebugUI.debugUI = b;
    }
}
