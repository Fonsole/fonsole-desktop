using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomDropdownEvent : UnityEvent<int> { }

[System.Serializable]
public class CustomDropdownOptionData {
    public string text;
    public int ID;
}

public class CustomDropdownEventContainer : MonoBehaviour {
    public List<CustomDropdownOptionData> dropdownOptions;
    public CustomDropdownEvent dropdownEvent;
}
