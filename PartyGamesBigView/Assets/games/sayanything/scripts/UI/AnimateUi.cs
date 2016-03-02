using UnityEngine;
using System.Collections;

public class AnimateUi : MonoBehaviour {

	public RectTransform rt {
		get{ return gameObject.GetComponent<RectTransform> (); }
	}

	public IEnumerator Move(Vector2 startPos, Vector2 midPos, Vector2 endPos, float t){
		rt.anchoredPosition = startPos;
		while (Vector2.Distance (midPos, rt.anchoredPosition) > 1f) {
			rt.anchoredPosition = Vector2.Lerp (rt.anchoredPosition, midPos, Time.deltaTime * t);
			yield return null;
		}
		rt.anchoredPosition = midPos;
		while (Vector2.Distance (endPos, rt.anchoredPosition) > 1f) {
			rt.anchoredPosition = Vector2.Lerp (rt.anchoredPosition, endPos, Time.deltaTime * t);
			yield return null;
		}
		rt.anchoredPosition = endPos;
	}

	public IEnumerator MovePause(Vector2 startPos, Vector2 midPos, Vector2 endPos, float t, float p){
		rt.anchoredPosition = startPos;
		while (Vector2.Distance (midPos, rt.anchoredPosition) > 1f) {
			rt.anchoredPosition = Vector2.Lerp (rt.anchoredPosition, midPos, Time.deltaTime * t);
			yield return null;
		}
		rt.anchoredPosition = midPos;
		yield return new WaitForSeconds(p);
		while (Vector2.Distance (endPos, rt.anchoredPosition) > 1f) {
			rt.anchoredPosition = Vector2.Lerp (rt.anchoredPosition, endPos, Time.deltaTime * t);
			yield return null;
		}
		rt.anchoredPosition = endPos;
	}
}
