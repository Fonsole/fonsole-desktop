using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PPlatform;

public class ControllerUi : MonoBehaviour
{
    public Text _Name;
    public Graphic[] _ColorTargetsImage;



    public void SetVisibility(bool val)
    {
        this.gameObject.SetActive(val);
    }

    public void SetContent(Controller c)
    {
        if (_Name != null)
            _Name.text = c.Name;

    }
}
