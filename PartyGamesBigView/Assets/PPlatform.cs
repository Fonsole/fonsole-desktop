using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Newtonsoft.Json;
using Luz.Helper;
using System.Collections.Generic;

public class Controller
{
    private int mId;
    public int Id
    {
        get { return mId; }
    }

    private string mName;
    public string Name
    {
        get { return mName; }
    }

    public Controller(int id, string name)
    {
        mId = id;
        mName = name;
    }


    public override string ToString()
    {
        return "Controller[id:" + mId + " name:" + mName + "]";
    }
}
public class PPlatform : MonoBehaviour
{
    private Netgroup mNetgroup = null;
    private string mActiveName = "gamelist";


    private Dictionary<int, Controller> mController = new Dictionary<int, Controller>();

    
    string TAG_CONTROLLER_REGISTER = "PLATFORM_CONTROLLER_REGISTER";
    struct ControllerRegisterMessage
    {
        public string name;
    }

    string TAG_CONTROLLER_DISCOVERY = "PLATFORM_CONTROLLER_DISCOVERY";
    private struct ControllerDiscoveryMessage
    {
        public int id;
        public string name;
    }

    string TAG_CONTROLLER_LEFT = "PLATFORM_CONTROLLER_LEFT";
    //no content

    
    string TAG_VIEW_DISCOVERY = "PLATFORM_VIEW_DISCOVERY";
    //no content

    string TAG_ENTER_GAME = "PLATFORM_ENTER_GAME";
    //string content

    string TAG_EXIT_GAME = "PLATFORM_EXIT_GAME";

    private struct PlatformMessage
    {
        public string tag;
        public string content;
    }

    private void Awake()
    {
        mNetgroup = GetComponent<Netgroup>();
    }

	private void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
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

    public void EnterGame(string name)
    {
        Send(TAG_ENTER_GAME, name, -1);
    }
	private void ShowGame(string name)
    {
        Debug.Log("Show game " + name);

    }

    private void HandlePlatformMessage(string tag, string content, int conId)
    {
        if (tag == TAG_CONTROLLER_REGISTER)
        {
            //send out discovery broadcast (which will be received locally too and handled

            ControllerRegisterMessage msg = JsonWrapper.FromJson<ControllerRegisterMessage>(content);
            Debug.Log("ControllerRegisterMessage received");
            ControllerDiscoveryMessage discoveryMsg = new ControllerDiscoveryMessage();
            discoveryMsg.id = conId;
            discoveryMsg.name = msg.name;
            Send(TAG_CONTROLLER_DISCOVERY, JsonWrapper.ToJson(discoveryMsg));

        }
        if(tag == TAG_CONTROLLER_DISCOVERY)
        {
            //add controller to the list
            ControllerDiscoveryMessage discoveryMsg = JsonWrapper.FromJson<ControllerDiscoveryMessage>(content);

            Controller c = new Controller(discoveryMsg.id, discoveryMsg.name);
            Debug.Log("Added new " + c);
            mController.Add(discoveryMsg.id, c);


            Send(TAG_ENTER_GAME, mActiveName, discoveryMsg.id);
        }

        
        //send the message out to the games

        if(tag == TAG_ENTER_GAME)
        {
            ShowGame(content);
        }

        if(tag == TAG_CONTROLLER_LEFT)
        {
            Debug.Log("Remove " + mController[conId]);
            //remove controller
            mController.Remove(conId);
        }

    }
    private void OnNetgroupMessageInternal(Netgroup.SignalingMessageType type, int conId, string content)
    {
        Debug.Log("Message type: " + type + " con id: " + conId + " content: " + content);
        if(type == Netgroup.SignalingMessageType.Connected)
        {
            GameObject go = GameObject.Find("PPlatformGui");
            PPlatformGui gui = go.GetComponent<PPlatformGui>();


            gui._gameCode.text = content;
        }
        else if (type == Netgroup.SignalingMessageType.UserJoined)
        {
            //send out the view discovery message to the new user so the controller knows how to contact the view
            Send(TAG_VIEW_DISCOVERY, "", conId);
        }
        else if (type == Netgroup.SignalingMessageType.UserLeft)
        {
            Send(TAG_VIEW_DISCOVERY, "", conId);
            HandlePlatformMessage(TAG_CONTROLLER_LEFT, null, conId);
        }
        else if (type == Netgroup.SignalingMessageType.UserMessage)
        {
            PlatformMessage pm = JsonWrapper.FromJson<PlatformMessage>(content);
            Debug.Log("Tag: " + pm.tag + " content " + pm.content + " raw json: " + content);
            HandlePlatformMessage(pm.tag, pm.content, conId);
        }




    }

    private void Send(string tag, string content, int lTo = -1)
    {
        PlatformMessage pm = new PlatformMessage();
        pm.tag = tag;
        pm.content = content;

        Debug.Log("Snd: [Tag:" + tag + " Content:" + content + " To:" + lTo + "]" );
        mNetgroup.SendMessageTo(JsonWrapper.ToJson(pm), lTo);
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
