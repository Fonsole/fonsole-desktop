using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPlatform.SayAnything.UI
{
    public class JudgingAndVotingScreen : GameScreen
    {

        public AnswerList _AnswerList;

        public void FixedUpdate()
        {
            if(_AnswerList != null)
            {
                return;
            }

        }

    }
}
