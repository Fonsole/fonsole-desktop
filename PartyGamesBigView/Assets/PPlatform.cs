using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class PPlatform : MonoBehaviour
{
    private Netgroup mNetgroup = null;

    private void Awake()
    {
        mNetgroup = GetComponent<Netgroup>();
    }

	private void Start ()
    {
        mNetgroup.Open(GetRandomKey(), OnNetgroupMessageInternal);
	}

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if(GUILayout.Button("Exit"))
        {
            mNetgroup.Close();
        }
        GUILayout.EndHorizontal();
    }
	
    private void OnNetgroupMessageInternal(Netgroup.SignalingMessageType type, int conId, string content)
    {

        Debug.Log("Message type:" + type + " con id:" + conId + " content" + content);
    }
    private string GetRandomKey()
    {

        StringBuilder result = new StringBuilder();
        for (var i = 0; i < 6; i++)
        {
            int charVal = 65 + UnityEngine.Random.Range(0, 24);
            result.Append((char)charVal);
        }
        return result.ToString();
    }
	private void Update () {
	
	}
}
