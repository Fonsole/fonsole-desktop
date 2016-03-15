using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MuteMusicUi : MonoBehaviour {

    /// <summary>
    /// Simple holder of mute on/off and hover sprites
    /// </summary>
    /// 
    public Sprite _targetOn;
    public Sprite _targetOff;
    public Sprite _targetHoverOn;
    public Sprite _targetHoverOff;

    private Button _muteMusicButton;
    private Image _muteMusicImage;
    private bool _muteToggle;

    private SpriteState _onState;
    private SpriteState _offState;

	void Awake () {
        _muteMusicButton = GetComponent<Button>();
        _muteMusicImage = GetComponent<Image>();

        _onState = new SpriteState();
        _onState.highlightedSprite = _targetHoverOn;
        _offState = new SpriteState();
        _offState.highlightedSprite = _targetHoverOff;
	}

    public void MuteToggle()
    {
        _muteToggle = _muteToggle ? false : true;
        if (_muteMusicButton)
        {
            _muteMusicButton.spriteState = _muteToggle ? _offState : _onState;
            _muteMusicImage.sprite = _muteToggle ? _targetOff : _targetOn;
        }
    }
}
