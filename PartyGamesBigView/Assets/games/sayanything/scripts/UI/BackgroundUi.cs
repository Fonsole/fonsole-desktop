using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace PPlatform.SayAnything.Ui
{
	public class BackgroundUi : MonoBehaviour
	{

		public Image background;
		public Image alternativeBackground;
		public Sprite[] backgrounds;
        private int current;
		private float progress;


		void Start () {
			alternativeBackground.color = new Color (1, 1, 1, 0);
            current = -1; progress = 1.1f;
            trySwitch();
		}

		void FixedUpdate()
		{
            trySwitch();
            Color bColor = background.color, abColor = alternativeBackground.color;
            bColor.a = 1 - progress;
            abColor.a = progress;
            background.color = bColor;
            alternativeBackground.color = abColor;
            progress += Time.deltaTime /300f;
		}

        private void trySwitch() {
            if (progress >= 1f) {
                current = (current + 1) % backgrounds.Length;
                int next = (current + 1) % backgrounds.Length;
                background.sprite = backgrounds[current];
                alternativeBackground.sprite = backgrounds[next];
                progress = 0f;
            }
        }

	}
}
