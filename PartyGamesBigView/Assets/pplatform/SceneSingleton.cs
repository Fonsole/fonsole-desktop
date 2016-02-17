using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PPlatform.Helper
{
    /// <summary>
    /// Singleton accesses an object that needs to be manually created in the scene + will be destroyed at the end of the scene
    /// (or manually)
    /// 
    /// In this case Instance will return null!!! 
    /// </summary>
    /// <typeparam name="T">A MonoBehaviour</typeparam>
    public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        return _instance;
                    }
                }

                return _instance;
                
            }
        }
    }
}
