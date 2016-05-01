using UnityEngine;
using System.Collections;
using PPlatform.SayAnything;
using PPlatform;

namespace DebugTools
{
    public class DebugUI : MonoBehaviour
    {
        void Awake()
        {
            DebugConsole.ActivateConsole();
            TL.ActivateLog();
            TL.ActivateEditorDirectJump();
            TL.LogTag(TL.TAG_ERROR);
            TL.LogTag(TL.TAG_WARNING);
            //TL.LogTag(TL.TAG_INFO);
            TL.LogTag(typeof(Platform).Name);
            
            string tags = "";
            foreach(string s in TL.VisibleTags)
            {
                tags += s + ", ";
            }
            TL.L("Log is active. Logging tags " + tags);
        }
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
        private bool debugUi = false;
        private void OnGUI()
        {

            GUILayout.BeginVertical();
            debugUi = GUILayout.Toggle(debugUi, "debugui");
            if (debugUi)
            {
                SayAnythingLogic logic = FindObjectOfType<SayAnythingLogic>();
                if (logic != null)
                    GUILayout.Label("state:" + logic.Data);

                if (Platform.Instance != null)
                {
                    GUILayout.Label("active Players:");
                    foreach (var v in Platform.Instance.ActiveControllers)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("    ");
                        GUILayout.Label(v.UserId + "\t" + v.Name);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndHorizontal();


            DebugConsole.DrawConsole();
        }
    }

}
