using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VoteDisplay : MonoBehaviour {
    public Image _OneVoteFigurine;
    public Image _TwoVotesFigurine;
    public Image _OneVoteIcon;
    public Image _TwoVotesIcon;

    private int _numVotes = 0;

    public int NumVotes
    {
        get { return _numVotes; }
        set
        {
            if (value != _numVotes)
            {
                _OneVoteFigurine.gameObject.SetActive(value == 1);
                _OneVoteIcon.gameObject.SetActive(value == 1);
                _TwoVotesFigurine.gameObject.SetActive(value == 2);
                _TwoVotesIcon.gameObject.SetActive(value == 2);

                _numVotes = value;
            }
        }
    }

    public Color Color
    {
        set
        {
            _OneVoteFigurine.color = value;
            _TwoVotesFigurine.color = value;
        }
    }

    public int VoteID;
}
