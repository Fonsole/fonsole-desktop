using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PPlatform;
using PPlatform.SayAnything.Ui;
using PPlatform.SayAnything;

public class ControllerUi : UserUi
{
    public Animator animator;

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
                animator.SetBool("join", false);
            }
            else
            {
                StartCoroutine(join(userId));
            }

        }
        mUserId = userId;
    }

    private IEnumerator join(int userId) {

        animator.SetBool("join", true);
        yield return null;

        yield return new WaitForSeconds(1.25f);

        //send out sound event
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnUserJoin();

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("joined"))
            yield return null;

        //switched to active
        //active user. change name and color
        SetColor(SayAnythingUi.Instance.GetUserColor(userId));
        SetUserName(SayAnythingUi.Instance.GetUserName(userId));
        SetJoinVisibile(false);
    }

}
