using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DebugTools
{
    public static class TL
    {
        public static readonly string TAG_ERROR = "error";
        public static readonly string TAG_WARNING = "warning";
        public static readonly string TAG_INFO = "info";
        private static bool showParameters = true;

        public static bool ShowParameters
        {
            get { return TL.showParameters; }
            set { showParameters = value; }
        } 

        private static bool sDoLog = false;
        private static MethodBase sUnityLog;

        /// <summary>
        /// Stores tags during log method calls. Just used to try to avoid creating garbage
        /// </summary>
        private static List<string> sCurrentTagList = new List<string>();

        private static List<String> visibleTags = new List<String>();

        public static String[] VisibleTags
        {
            get { return TL.visibleTags.ToArray(); }
        }

        public delegate void CustomFormatter(string msg, List<string> tags, bool isError, bool isWarning, StackFrame[] stackFrames, StringBuilder message);

        private static CustomFormatter sFormatter = null;

        public static void SetCustomFormatter(CustomFormatter formatter)
        {
            sFormatter = formatter;
        }

        public static void ActivateLog()
        {
            sDoLog = true;
        }
        public static void ActivateEditorDirectJump()
        {
            if (sUnityLog == null)
                sUnityLog = typeof(UnityEngine.Debug).GetMethod("LogPlayerBuildError", BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static void LogTag(string tag)
        {
            visibleTags.Add(tag);
        }


        public static void L(string msgOrObject, params string[] tags)
        {
            if (sDoLog == false)
                return;
            sCurrentTagList.Clear();
            sCurrentTagList.AddRange(tags);
            sCurrentTagList.Add(TAG_INFO);
            LogList(msgOrObject, sCurrentTagList);
        }
        public static void LW(string msgOrObject, params string[] tags)
        {
            if (sDoLog == false)
                return;
            sCurrentTagList.Clear();
            sCurrentTagList.AddRange(tags);
            sCurrentTagList.Add(TAG_WARNING);
            LogList(msgOrObject, sCurrentTagList);
        }
        public static void LE(string msgOrObject, params string[] tags)
        {
            if (sDoLog == false)
                return;
            sCurrentTagList.Clear();
            sCurrentTagList.AddRange(tags);
            sCurrentTagList.Add(TAG_ERROR);
            LogList(msgOrObject, sCurrentTagList);
        }
        
        public static void LogList(string msg, List<string> tags)
        {

            StackTrace stackTrace = new StackTrace(true);
            StackFrame[] stackFrames = stackTrace.GetFrames();
            Type declaringType = null;
            string file = "";
            int line = 0;
            int col = 0;
            GetCallerInfo(stackFrames, out declaringType, out file, out line, out col);

            if(declaringType != null)
            {
                tags.Add(declaringType.Name);
            }
            if (sDoLog == false || visibleTags.Intersect(tags).Any() == false)
            {
                return;
            }
            bool isError = tags.Contains(TAG_ERROR);
            bool isWarning = tags.Contains(TAG_WARNING);
            StringBuilder message = new StringBuilder();
            if (sFormatter == null)
            {
                FormatForEditorLog(msg, tags, isError, isWarning, stackFrames, message);
            }
            else
            {
                sFormatter(msg, tags, isError, isWarning, stackFrames, message);   
            }

            //work around to allow jumping into code via unity editor
            if (sUnityLog != null)
            {
                sUnityLog.Invoke(null, new object[] { message.ToString(), file, line, col });
            }else if(isError)
            {
                UnityEngine.Debug.LogError(message.ToString());
            }else if(isWarning)
            {
                UnityEngine.Debug.LogWarning(message.ToString());
            }
            else
            {
                UnityEngine.Debug.Log(message.ToString());
            }
            
        }

        private static void FormatForEditorLog(string msg, List<string> tags, bool isError, bool isWarning, StackFrame[] stackFrames, StringBuilder message)
        {
            //no tags? just print plain
            if (tags == null || tags.Count == 0)
            {
                message.Append(msg);
            }
            else
            {
                //print tags
                message.Append("<color=green><size=9>");
                for (int k = 0; k < tags.Count; k++)
                {
                    message.Append(tags[k]);
                    if (k + 1 < tags.Count)
                        message.Append("|");
                }
                message.Append("</size></color>");

                //print the color tag dependend
                if (isError)
                {
                    isError = true;
                    message.Append("<b><color=red>");
                    message.Append(msg);
                    message.Append("</color></b>");
                }
                else if (isWarning)
                {
                    isWarning = true;
                    message.Append("<b><color=#555500FF>");
                    message.Append(msg);
                    message.Append("</color></b>");
                }
                else
                {
                    message.Append(msg);
                }
            }



            //look for the first method call in the stack that isn't from this class.
            //save the first one to jump into it later and add all further lines to the log
            for (int i = 0; i < stackFrames.Length; i++)
            {
                MethodBase mb = stackFrames[i].GetMethod();

                //ignore stackframes inside the logger itself
                if (mb.DeclaringType != typeof(TL))
                {

                    message.Append("\n");
                    message.Append("<color=blue>");
                    message.Append(mb.DeclaringType.FullName);
                    message.Append(":");
                    message.Append(mb.Name);
                    message.Append("(");
                    if (showParameters)
                    {
                        ParameterInfo[] paramters = mb.GetParameters();
                        for (int k = 0; k < paramters.Length; k++)
                        {
                            message.Append(paramters[k].ParameterType.Name);
                            if (k + 1 < paramters.Length)
                                message.Append(", ");
                        }
                    }

                    message.Append(")");
                    message.Append("</color>");


                    message.Append(" (at ");
                    //the first stack message is found now we add the other stack frames to the log
                    message.Append(FormatFileName(stackFrames[i].GetFileName()));
                    message.Append(":");
                    message.Append(stackFrames[i].GetFileLineNumber());
                    message.Append(")");
                    message.Append("\n");
                }
            }
        }

        private static bool GetCallerInfo(StackFrame[] stackFrames, out Type declaringType, out string file, out int line, out int col)
        {
            for(int i = 0; i < stackFrames.Length; i++)
            {
                MethodBase mb = stackFrames[i].GetMethod();
                if (mb.DeclaringType != typeof(TL))
                {
                    declaringType = stackFrames[i].GetMethod().DeclaringType;
                    file = FormatFileName(stackFrames[i].GetFileName());
                    line = stackFrames[i].GetFileLineNumber();
                    col = stackFrames[i].GetFileColumnNumber();
                    return true;
                }
            }
            declaringType = null;
            file = "File not found!";
            line = 0;
            col = 0;
            return false;
        }
        private static string FormatFileName(String file)
        {
            if (file != null && file.Contains("Assets"))
            {
                //remove everything of the absolute path that is before the Assetfolder
                //using the destination of the Assetfolder to get the right length (not ideal)
                return file.Remove(0, Application.dataPath.Length - "Assets".Length);
            }
            else if (file != null)
            {
                return file;
            }
            else
            {
                return "unknown";
            }

        }
    }
}
