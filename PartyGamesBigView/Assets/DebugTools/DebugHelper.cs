﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DebugTools
{
    public class DebugConsole
    {
        private static bool sDebugMode = true;
        public static bool DebugMode
        {
            get { return DebugConsole.sDebugMode; }
            set { DebugConsole.sDebugMode = value; }
        }
        public static bool sShowConsole = false;
        public static bool sConsoleAutoScroll = true;



        private static Vector2 mDebugConsoleScrollPos = new Vector2();
        private static StringBuilder mLog = null;
        private static int mLines = 0;
        public static void ActivateConsole()
        {
            if (mLog != null)
                return;
            mLog = new StringBuilder();
            Application.logMessageReceived += LogType;


            //Application.RegisterLogCallback(LogType);
        }
        private static void LogType(string condition, string stackTrace, LogType type)
        {
            int lines = 0;
            mLog.Append(condition);
            if (type == UnityEngine.LogType.Exception)
            {
                lines += stackTrace.Count((x) => { return x == '\n'; });
                mLog.Append(stackTrace);
            }
            mLog.Append("\n");
            lines++;

            mLines += lines;

            int foundLines = 0;
            for (int i = mLog.Length - 1; i >= 0; i--)
            {
                if (mLog[i] == '\n')
                {
                    foundLines++;
                    if (foundLines  == 100)
                    {
                        mLog.Remove(0, i + 1);
                        break;
                    }
                }
            }
        }

        public static void Update()
        {

            if (Input.GetKeyUp(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))
            {
                sDebugMode = !sDebugMode;
            }
        }
        public static void DrawConsole()
        {




            if (mLog == null)
                return;;

            if (sDebugMode == false)
                return;


            GUILayout.BeginArea(new Rect(Screen.width - 150, 140, 140, 200));
            GUILayout.BeginHorizontal();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (sShowConsole == false)
            {
                if (GUI.Button(new Rect(Screen.width - 40, Screen.height - 20, 40, 20), "Show"))
                {
                    sShowConsole = true;
                }
            }
            else
            {
                if (GUI.Button(new Rect(Screen.width - 40, Screen.height * 0.75f - 20, 40, 20), "Hide"))
                {
                    //Debug.Log(mDebugConsoleScrollPos);
                    sShowConsole = false;
                }
                if (GUI.Button(new Rect(Screen.width - 90, Screen.height * 0.75f - 20, 40, 20), "Auto"))
                {
                    sConsoleAutoScroll = !sConsoleAutoScroll;
                    //Debug.Log(mDebugConsoleScrollPos);
                    //mShowConsole = false;
                }


                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = Color.white;
                textStyle.richText = true;
                GUILayout.BeginArea(new Rect(0, Screen.height * 0.75f, Screen.width, Screen.height * 0.25f));
                mDebugConsoleScrollPos = GUILayout.BeginScrollView(mDebugConsoleScrollPos);
                if(sConsoleAutoScroll)
                    mDebugConsoleScrollPos = new Vector2(0, 1000000000);
                GUILayout.TextArea(mLog.ToString(), textStyle);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }

        }

    }
}
