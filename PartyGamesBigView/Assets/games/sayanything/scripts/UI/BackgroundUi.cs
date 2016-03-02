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

		private GameState prevState;
		private float target = 1;

		void Start (){
			alternativeBackground.color = new Color (1, 1, 1, 0);
		}

		void FixedUpdate()
		{
			SharedData data = SayAnythingUi.Instance.CurrentData;
			switch (data.state) {
				case GameState.WaitForStart:
					background.sprite = backgrounds [0];
					break;
				case GameState.Questioning:
					alternativeBackground.sprite = backgrounds [2];
					break;
				case GameState.Answering:
					background.sprite = backgrounds [2];
					break;
				case GameState.JudgingAndVoting:
					alternativeBackground.sprite = backgrounds [1];
					break;
				case GameState.ShowWinner:
					background.sprite = backgrounds [1];
					break;
				case GameState.ShowScore:
					alternativeBackground.sprite = backgrounds [1];
					break;
			}

			if (data.state != prevState) {
				prevState = data.state;
				target = Mathf.Abs (target - 1);
			}

			background.color = Color.Lerp (background.color, new Color (1, 1, 1, target), Time.deltaTime * 2f);
			alternativeBackground.color = Color.Lerp (alternativeBackground.color, new Color (1, 1, 1, Mathf.Abs(target-1)), Time.deltaTime * 2f);
		}
	}
}
