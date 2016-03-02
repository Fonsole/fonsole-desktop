using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PPlatform;
using PPlatform.SayAnything.Ui;
using PPlatform.SayAnything;

public class ControllerUi : UserUi
{
    private int mUserId = SharedData.UNDEFINED;
    
    public void Refresh(int userId, SharedData data)
    {

        if (mUserId != userId)
        {
            //only call methods if something changed
            if (userId == SharedData.UNDEFINED)
            {
                //inactive user -> set to default design
                SetDefault();
            }
            else
            {
                //switched to active
                //active user. change name and color
                SetColor(SayAnythingUi.Instance.GetUserColor(userId));
                SetUserName(SayAnythingUi.Instance.GetUserName(userId));
				SetJoinVisibile (false);

                //send out sound event
                if (AudioManager.Instance != null)
                    AudioManager.Instance.OnUserJoin();
            }



        }
        mUserId = userId;
    }
}
