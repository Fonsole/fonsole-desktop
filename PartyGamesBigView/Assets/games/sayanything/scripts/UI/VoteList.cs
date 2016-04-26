using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PPlatform.SayAnything;
using System.Collections.Generic;

namespace PPlatform.SayAnything.Ui
{
    public class VoteList : MonoBehaviour {

        public Image[] _VoteImages;
        public VoteDisplay[] _VoteDisplays;

        public void Refresh(int userId, SharedData data)
        {
            //Since refresh is called every frame, disassociate the vote displays
            //to make sure that the leftmost display is always the first shown
            for (int i = 0; i < _VoteDisplays.Length; ++i)
            {
                _VoteDisplays[i].NumVotes = 0;
                _VoteDisplays[i].VoteID = -1;
                _VoteDisplays[i].gameObject.SetActive(false);
            }

            List <int> votes = data.GetVotes(userId);
            int counter = 0;
            for(; counter < votes.Count; counter ++)
            {
                VoteDisplay display = DisplayForID(votes[counter]);
                if (display != null)
                    display.NumVotes++;
            }
        }

        private VoteDisplay DisplayForID(int playerID)
        {
            int len = _VoteDisplays.Length;
            int firstInactive = 0;
            for (int i = 0; i < len; ++i)
            {
                if (_VoteDisplays[i].gameObject.activeSelf)
                {
                    if (_VoteDisplays[i].VoteID == playerID)
                        return _VoteDisplays[i];
                    ++firstInactive;
                }
            }
            _VoteDisplays[firstInactive].VoteID = playerID;
            _VoteDisplays[firstInactive].gameObject.SetActive(true);
            _VoteDisplays[firstInactive].Color = SayAnythingUi.Instance.GetUserColor(playerID);
            return _VoteDisplays[firstInactive];
        }
    }
}