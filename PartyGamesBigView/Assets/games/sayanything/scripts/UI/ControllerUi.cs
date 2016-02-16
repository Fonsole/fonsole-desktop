using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PPlatform;
using PPlatform.SayAnything.Ui;
using PPlatform.SayAnything;

public class ControllerUi : UserUi
{

    
    public void Refresh(int userId, SharedData data)
    {
        if (userId == SharedData.UNDEFINED)
        {
            //inactive user -> set to default design
            SetDefault();
        }
        else
        {
            //active user. change name and color
            SetColor(SayAnythingUi.Instance.GetUserColor(userId));
            SetUserName(SayAnythingUi.Instance.GetUserName(userId));
        }
    }
}
