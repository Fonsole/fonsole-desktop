using DebugTools;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameListUI : MonoBehaviour {

    public static readonly int CONTENT_ID_HOME = 0;
    public static readonly int CONTENT_ID_HOST = 1;
    public static readonly int CONTENT_ID_JOIN = 2;
    public static readonly int CONTENT_ID_LIST = 3;
    public static readonly int CONTENT_ID_SHOP = 4;
    public static readonly int CONTENT_ID_COMMUNITY = 5;
    public static readonly int CONTENT_ID_SETTINGS = 6;
    public static readonly int CONTENT_ID_GAME_VIEW = 7;

    public List<RectTransform> contentList;

    private int currentContent = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnHideComplete(Tween t, GameObject go)
    {
        t.Rewind();
        go.SetActive(false);
    }

    public void OnShowContent(int contentID)
    {
        Debug.Log(contentID);
        contentList[contentID].gameObject.SetActive(true);
        contentList[contentID].DOAnchorPosX(Screen.width, 0.5f).From().SetEase(Ease.OutQuad);

        RectTransform oldContent = contentList[currentContent];
        Tween hideTween = oldContent.DOAnchorPosX(-Screen.width, 0.5f);
        hideTween.SetEase(Ease.OutQuad);
        hideTween.OnComplete(() => OnHideComplete(hideTween, oldContent.gameObject));

        currentContent = contentID;
    }
}
