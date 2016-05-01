using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PPlatform;
using PPlatform.SayAnything.Ui;
using PPlatform.SayAnything;
using DebugTools;

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
            else if (mUserId == SharedData.UNDEFINED)
            {
                //new user
                StartCoroutine(join(userId));
            }
            else
            {
                //user got replaced by another user that logged out -> just change the icon without animation
                //the animation never called SwitchActive so it just got stuck showing the old user
                //change it already before this call so it updates the correct user
                mUserId = userId;
                SwitchActive();
            }

        }
        mUserId = userId;
    }

    private IEnumerator join(int userId) {

        TL.L("animating = true", UserUi.LOGTAG);
        animating = true;
        animator.SetBool("join", true);
        yield return null;
        
        /* old code using enumeration for some things, had bugs may come back to later

        yield return new WaitForSeconds(1.3f);

        Color current = _UsernameText.color;
        current.a = 0f;

        //switched to active
        //active user. change name and color
        SetColor(current);
        SetUserName(SayAnythingUi.Instance.GetUserName(userId));
        _UsernameParent.SetActive(true);

        //send out sound event
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnUserJoin();

        float timer = 0f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("joined")) {
            if (timer < 1f) {
                current.a = Mathf.Lerp(0f, 1f, timer);
                _UsernameText.color = current;
                timer += Time.deltaTime;
            }
            yield return null;
        }
        animating = false;

        //switched to active
        //active user. change name and color
        SetColor(SayAnythingUi.Instance.GetUserColor(userId));
        SetJoinVisibile(false);

        while (timer < 1f) {
            current.a = Mathf.Lerp(0f, 1f, timer);
            _UsernameText.color = current;
            timer += Time.deltaTime;
        }
        */
    }

    // called from anim event in joining.anim
    private void SwitchActive()
    {
        TL.L("SwitchActive", UserUi.LOGTAG);
        Color current = _UsernameText.color;

        SetColor(current);
        SetUserName(SayAnythingUi.Instance.GetUserName(mUserId));
        _UsernameParent.SetActive(true);

        //send out sound event
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnUserJoin();

        TL.L("animating = false", UserUi.LOGTAG);
        animating = false;


        TL.L("Set user " + SayAnythingUi.Instance.GetUserName(mUserId) + " color" + SayAnythingUi.Instance.GetUserColor(mUserId), UserUi.LOGTAG);
        //switched to active
        //active user. change name and color
        SetColor(SayAnythingUi.Instance.GetUserColor(mUserId));
        SetJoinVisibile(false);
    }

}
