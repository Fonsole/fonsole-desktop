using UnityEngine;
using System.Collections;

namespace PPlatform.Helper
{
    public class VisitURL : MonoBehaviour
    {
        public string URL;

        public void NavigateToURL()
        {
            Application.OpenURL(URL);
        }
    }

}