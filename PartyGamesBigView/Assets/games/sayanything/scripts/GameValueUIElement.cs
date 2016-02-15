using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// A value in the game. Can be either a specific ui element e.g. the player boxes at the start screen or
/// generic stuff e.g. score value. The set methods can be updated or changed for specific types later.
/// 
/// e.g. SetVisibility could play an animation to blend it in / out
/// </summary>
public class GameValueUIElement : MonoBehaviour
{
    private Text _TextSubElement;

    public void SetValue(string value)
    {
        if(_TextSubElement != null)
        {
            _TextSubElement.text = value;
        }
    }

    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
