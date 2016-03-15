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

        animating = true;
        animator.SetBool("join", true);
        yield return null;

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
    }

}
