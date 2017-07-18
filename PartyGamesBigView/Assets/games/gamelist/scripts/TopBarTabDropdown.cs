using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CustomDropdownEventContainer))]
public class TopBarTabDropdown : Dropdown {
    
    private Text textComponent;

    public int IDOffset = 1;

    void CustomListener(int i)
    {
        GetComponent<CustomDropdownEventContainer>().dropdownEvent.Invoke(GetComponent<CustomDropdownEventContainer>().dropdownOptions[i].ID);
    }

    void Start()
    {
        textComponent = GetComponentInChildren<Text>();
        targetGraphic = textComponent ? textComponent : GetComponentInChildren<Graphic>();

        onValueChanged = new DropdownEvent();
        onValueChanged.AddListener(CustomListener);

        options.Clear();
        List<OptionData> newOptions = new List<OptionData>();
        foreach (var o in GetComponent<CustomDropdownEventContainer>().dropdownOptions)
        {
            newOptions.Add(new OptionData(o.text));
        }
        AddOptions(newOptions);
    }

    void Update()
    {

    }
}
