using UnityEngine;
using System.Collections;

namespace PPlatform.SayAnything.Ui
{
    public class ScreenTransition : MonoBehaviour
    {
        public delegate void ShowScreenDelegate(GameObject target, GameState targetState);
        public ShowScreenDelegate ShowScreen;
        public float DelayTime;

        //NOTE: This is just a temporary solution, to delay the showing of the 
        //next screen. TransitionOut should probably trigger the tweening of 
        //elements off the screen, and dispatch ShowScreen at the end
        public void TransitionOut(GameObject nextScreen, GameState nextState)
        {
            StartCoroutine(TransitionCoroutine(nextScreen, nextState));
        }

        IEnumerator TransitionCoroutine(GameObject nextScreen, GameState nextState)
        {
            yield return new WaitForSeconds(DelayTime);
            ShowScreen(nextScreen, nextState);
        }
    }
}