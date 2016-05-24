using UnityEngine;
using System.Collections;


namespace PPlatform.SayAnything.Message
{
    public struct SharedDataUpdate
    {
        public static readonly string TAG = "SayAnything_SharedDataUpdate";
        public SharedData sharedData;

        public SharedDataUpdate(SharedData lSharedData)
        {
            this.sharedData = lSharedData;
        }
    }

    public struct StartGame
    {
        public static readonly string TAG = "SayAnything_StartGame";
        //no content
    }
    public struct GameLoaded
    {
        public static readonly string TAG = "SayAnything_GameLoaded";
        //no content
    }

    public struct Rules
    {
        public static readonly string TAG = "SayAnything_SkipRules";
        //skip rules screen
    }
    //sent after the judge chooses a question
    public struct Question
    {
        public static readonly string TAG = "SayAnything_Question";
        public string question;

        public Question(string lQuestion)
        {
            this.question = lQuestion;
        }
    }

    //sent after the controllers enter an answer and press the confirm button
    public struct Answer
    {
        public static readonly string TAG = "SayAnything_Answer";

        public string answer;

        public Answer(string lAnswer)
        {
            this.answer = lAnswer;
        }
    }

    public struct Judge
    {
        public static readonly string TAG = "SayAnything_Judge";

        public int playerId;

        public Judge(int lPlayerId)
        {
            this.playerId = lPlayerId;
        }

    }


    public struct Vote
    {
        public static readonly string TAG = "SayAnything_Vote";

        public int votePlayerId1;
        public int votePlayerId2;

        public Vote(int lVotePlayerId1, int lVotePlayerId2)
        {
            this.votePlayerId1 = lVotePlayerId1;
            this.votePlayerId2 = lVotePlayerId2;
        }
    }

    public struct ShowCustom
    {
        public static readonly string TAG = "SayAnything_ShowCustom";
        //show custom answer
    }
}
