using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TopBarTabButton : Selectable {

    private Text textComponent;

	void Start () {
        textComponent = GetComponentInChildren<Text>();
        targetGraphic = textComponent ? textComponent : GetComponentInChildren<Graphic>();
    }
	
	// Update is called once per frame
	void Update () {

    }
}
