using DebugTools;
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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnShowContent(int contentID)
    {
        Debug.Log("GUI: Open content ID " + contentID);
    }
}
