using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPlatform.SayAnything
{     
    
    public enum GameState
    {
        WaitForStart = 0,
        Questioning = 1,
        Answering = 2,
		DisplayAnswers = 3,
        //ShowAnswers = 3, //show answers will be merged with judging (can be hidden via controller logic but isn't essential for the game logic
        JudgingAndVoting = 4,
        //Voting = 5, //voting is merged with Judging
        ShowWinner = 6,
        ShowScore = 7
    }


    public class SharedData
    {
        public const int UNDEFINED = -1;



        public GameState state = SayAnything.GameState.WaitForStart;
        
        //user id of the judge. only the connection id for now
        public int judgeUserId = UNDEFINED;
        
        //simply the text of the question
        public string question = null;
        
        //will contain the player id as key and then the answer.
        public Dictionary<int, string> answers = new Dictionary<int, string>();
        
        //user id of the answer the judge has chosen
        public int judgedAnswerId = UNDEFINED;
        
        //key: user id (equals the id the answer of this user has in the "answers" object)
        //value: a list of user ids the vote came from (needed to show the color badges in the end)
        //(move to local data, view only?)
        public Dictionary<int, List<int>> votes = new Dictionary<int, List<int>>();
        
        //scores in this round (move to local data, view only?)
        public Dictionary<int, int> roundScore = new Dictionary<int, int>();
        
        //scores overall (move to local data, view only?)
        public Dictionary<int, int> totalScore = new Dictionary<int, int>();

        public float timeLeft = 30;
         
         //functions to easily fill and read the data (ideall this should be done only via functions later to prevent bugs)
        public void CancelVotesBy(int lFrom)
        {
            foreach(var v in this.votes)
            {
                v.Value.Remove(lFrom);
                v.Value.Remove(lFrom);
            }
        }
        public void AddVote(int lFrom, int lTo)
        {
            if(this.votes.ContainsKey(lTo))
            {
                //user got at least one vote already -> add the new vote
                this.votes[lTo].Add(lFrom);
            }else
            {
                //user didn't get a vote yet -> add a list with one vote
                this.votes[lTo] = new List<int>(new int[]{lFrom});

            }
        }
         

        public List<int> GetVotes(int lUserId)
        {
            if(this.votes.ContainsKey(lUserId))
            {
                return this.votes[lUserId];
            }
            else{
                 
                return new List<int>(); //empty list. user never received a vote.
            }
        }
         
         
        public void resetRoundData()
        {
            this.state = SayAnything.GameState.WaitForStart;

            this.judgeUserId = -1;
            this.question = null;
            this.answers = new Dictionary<int,string>();
            this.judgedAnswerId = -1;
            this.votes = new Dictionary<int,List<int>>();
            this.roundScore = new Dictionary<int, int>();
        }

        /// <summary>
        /// Awards score to a user. Only valid for the current round.
        /// 
        /// Value can't be higher than 3.
        /// </summary>
        /// <param name="lUserId"></param>
        /// <param name="lPoints"></param>
        public void awardRoundScore(int lUserId, int lPoints)
        {
            //add it to the score received this round
            if(this.roundScore.ContainsKey(lUserId))
            {
                this.roundScore[lUserId] += lPoints;
            }else{
                this.roundScore[lUserId] = lPoints;
            }

            if (this.roundScore[lUserId] > 3)
                this.roundScore[lUserId] = 3;
        }

        public void awardTotalScore(int lUserId, int lPoints)
        {
            //add it to the total score
            if (this.totalScore.ContainsKey(lUserId))
            {
                this.totalScore[lUserId] += lPoints;
            }
            else
            {
                this.totalScore[lUserId] = lPoints;
            }
        }


        public override string ToString()
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("Shared Data: ");

            st.Append("\t");
            st.Append("state:\t");
            st.AppendLine("" + state);


            st.Append("\t");
            st.Append("judgeUserId:\t");
            st.AppendLine("" + judgeUserId);


            st.Append("\t");
            st.Append("question:\t");
            st.AppendLine("" + question);


            st.Append("\t");
            st.Append("answers:\n");
            foreach (var v in answers)
            {
                st.Append("\t\t");
                st.Append(v.Key);
                st.Append(":");
                st.Append(v.Value);
                st.AppendLine();
            }
            st.AppendLine();


            st.Append("\t");
            st.Append("judgedAnswerId:\t");
            st.AppendLine("" + judgedAnswerId);

            st.Append("\t");
            st.Append("votes:\n");
            foreach (var v in votes)
            {
                st.Append("\t\t");
                st.Append(v.Key);
                st.Append(":");
                st.Append("|");
                v.Value.ForEach((x) => { st.Append(x); st.Append("|"); });
                
                st.AppendLine();
            }
            st.AppendLine();


            st.Append("\t");
            st.Append("score:\n");
            foreach (var v in roundScore)
            {
                st.Append("\t\t");
                st.Append(v.Key);
                st.Append(":");
                st.Append(v.Value);
                st.AppendLine();
            }
            st.AppendLine();


            st.Append("\t");
            st.Append("total score:\n");
            foreach (var v in totalScore)
            {
                st.Append("\t\t");
                st.Append(v.Key);
                st.Append(":");
                st.Append(v.Value);
                st.AppendLine();
            }
            st.AppendLine();


            st.Append("\t");
            st.Append("timeLeft: \t");
            st.Append(this.timeLeft);
            return st.ToString();
        }


        public static IEnumerable<GameState> GetValidStates()
        {
            return Enum.GetValues(typeof(GameState)).Cast<GameState>();
        }
    }
}
