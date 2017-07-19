using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DynamicGrid : MonoBehaviour {

    public int columns = 4;

    private float ratio = 16f / 9f;

	void Start () {

		RectTransform parent = gameObject.GetComponent<RectTransform> ();
		GridLayoutGroup grid = gameObject.GetComponent<GridLayoutGroup> ();

        var actualWidth = (parent.rect.width - grid.padding.right - grid.padding.left - (grid.spacing.x * (columns - 1))) / columns;

		grid.cellSize = new Vector2 (actualWidth, actualWidth / ratio);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
