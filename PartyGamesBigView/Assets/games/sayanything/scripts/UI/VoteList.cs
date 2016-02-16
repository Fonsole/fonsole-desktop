using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PPlatform.SayAnything;
using System.Collections.Generic;

namespace PPlatform.SayAnything.UI
{
    public class VoteList : MonoBehaviour {

        public Image[] _VoteImages;


	    // Use this for initialization
	    void Start () {
	        if(_VoteImages == null)
            {
                Debug.Log(this.name + " is missing references! ");
                this.gameObject.SetActive(false);
            }
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }

        public void Refresh(int userId, SharedData data)
        {
            List<int> votes = data.GetVotes(userId);
            int counter = 0;
            for(; counter < votes.Count; counter ++)
            {
                if(_VoteImages.Length > counter)
                {
                    _VoteImages[counter].color = SayAnythingUi.Instance.GetUserColor(votes[counter]);
                    _VoteImages[counter].gameObject.SetActive(true);
                }
            }
            for (; counter < _VoteImages.Length; counter++)
            {
                _VoteImages[counter].gameObject.SetActive(false);
            }
        }
    }


}