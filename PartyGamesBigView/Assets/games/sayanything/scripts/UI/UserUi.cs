using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PPlatform.SayAnything.Ui
{

    /// <summary>
    /// Base class for everything that can contain user specific data.
    /// </summary>
    public abstract class UserUi : MonoBehaviour
    {
		public AnimateUi anim;

        /// <summary>
        /// Parent of the username field. In some states this is set to inactive to hide the username
        /// </summary>
        public GameObject _UsernameParent;
		public GameObject _JoiningParent;

        /// <summary>
        /// Text of the username
        /// </summary>
        public Text _UsernameText;

        /// <summary>
        /// Image objects that will be tinted based on the color of the user
        /// (white while username invisible)
        /// </summary>
        public Image[] _UsernameColors;
        private Color[] mDefaultColors;


        private Color mCurrentColor = Color.white;

        public bool animating = false;

        protected string mDefaultUsernameText;

        protected virtual void Awake()
        {
			anim = gameObject.AddComponent<AnimateUi> ();
            mDefaultUsernameText = _UsernameText.text;
            mDefaultColors = new Color[_UsernameColors.Length];
            for(int i = 0; i < mDefaultColors.Length; i++)
            {
                mDefaultColors[i] = _UsernameColors[i].color;
            }
        }


        protected void SetUserName(string username)
        {
            if (_UsernameText.text != username)
                _UsernameText.text = username;
        }

        /// <summary>
        /// Changes the color of the elements
        /// </summary>
        /// <param name="c"></param>
        protected void SetColor(Color c)
        {
            if (mCurrentColor != c)
            {
                mCurrentColor = c;
                if (_UsernameColors != null)
                {
                    for (int i = 0; i < _UsernameColors.Length; i++)
                    {
                        Color colorWithTransparency = c;
                        //change the color depending on the transparency of the element
                        colorWithTransparency.a = _UsernameColors[i].color.a;
                        _UsernameColors[i].color = colorWithTransparency;
                    }
                }
            }
        }

        protected void SetDefault()
        {
            if (animating) return;

            mCurrentColor = Color.white;
            ResetColors();
            SetUserName(mDefaultUsernameText);
			SetJoinVisibile (true);
        }

        protected void ResetColors()
        {
            for (int i = 0; i < mDefaultColors.Length; i++)
            {
                if (_UsernameColors[i].color != mDefaultColors[i])
                    _UsernameColors[i].color = mDefaultColors[i];
            }
        }

        /// <summary>
        /// Makes the element visible or invisible. This is polled.
        /// 
        /// Animations can be started if this switches to visible instead of making it visible immediately.
        /// </summary>
        /// <param name="isVisible">true for visible, false for hidding</param>
        protected void SetVisibile(bool isVisible)
        {
            //This method might be polled. Only change the visibility if necessary
            if (isVisible != gameObject.activeSelf)
            {
                gameObject.SetActive(isVisible);
            }
        }

		protected void SetJoinVisibile(bool isVisible)
		{
            if (animating) return;

			//This method might be polled. Only change the visibility if necessary
			if (isVisible != _JoiningParent.activeSelf)
			{
				_JoiningParent.SetActive(isVisible);
				_UsernameParent.SetActive(!isVisible);
			}
		}

        
    }
}
