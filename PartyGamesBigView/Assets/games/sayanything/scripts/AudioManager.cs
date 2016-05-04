using UnityEngine;
using System.Collections;
using PPlatform.Helper;

public class AudioManager : SceneSingleton<AudioManager>
{
    public AudioClip _OnUserJoin;
    public AudioClip _BackgroundMusic;
    public AudioClip[] _OnQuestionSelected;
    public AudioClip[] _OnAnswerSubmitted;
    private AudioClip[] _OnAnswerSubmittedShuffled;
    private int _LastAnswerClipPlayed = 0;

    public AudioSource _SoundSource;
    public AudioSource _MusicSource;

    private bool _MuteToggle;

	void Start (){
        AudioListener.volume = 0.50f;
		PlayMusic(_BackgroundMusic);

        _OnAnswerSubmittedShuffled = new AudioClip[_OnAnswerSubmitted.Length];
        ShuffleAnswerSubmittedClips();
	}

    public void MuteToggle()
    {
        _MuteToggle = _MuteToggle ? false : true;
        AudioListener.pause = _MuteToggle ? true : false;
    }

    public void OnUserJoin()
    {
        PlaySound(_OnUserJoin);

    }
	public void OnWaitForStart (){
		PlayMusic(_BackgroundMusic);
	}
	public void OnStartGame()
	{
		StartCoroutine(FadeMusic(0));
	}
    public void OnQuestionSelected()
    {
        PlaySound(_OnQuestionSelected[Random.Range(0, _OnQuestionSelected.Length)]);
    }
    public void OnAnswerSubmitted()
    {
        int current = _LastAnswerClipPlayed++;
        if (current >= _OnAnswerSubmittedShuffled.Length)
        {
            ShuffleAnswerSubmittedClips();
            current = 0;
        }

        PlaySound(_OnAnswerSubmittedShuffled[current]);
    }
    private void ShuffleAnswerSubmittedClips()
    {
        int len = _OnAnswerSubmittedShuffled.Length;
        for (int i = 0; i < len; ++i)
        {
            _OnAnswerSubmittedShuffled[i] = _OnAnswerSubmitted[i];
        }

        //cheap and easy array shuffle
        while (len > 1)
        {
            int next = Random.Range(0, len);
            --len;

            AudioClip temp = _OnAnswerSubmittedShuffled[next];
            _OnAnswerSubmittedShuffled[next] = _OnAnswerSubmittedShuffled[len];
            _OnAnswerSubmittedShuffled[len] = temp;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        //todo: if multiple sounds need to be played at the same time we can schedule multiple audio soruces here
        if(clip != null)
        {
            _SoundSource.PlayOneShot(clip);
        }
    }
    private void PlayMusic(AudioClip clip)
    {
        //todo: if multiple sounds need to be played at the same time we can schedule multiple audio soruces here
        if (clip != null && !_MusicSource.isPlaying)
        {
            _MusicSource.PlayOneShot(clip);
			StartCoroutine(FadeMusic(1f));
        }
    }
    //todo: fix audio fade for full/no volume
	private IEnumerator FadeMusic (float target){
		_MusicSource.volume = Mathf.Round(Mathf.Abs (target - 1f));
		while (Mathf.Abs(_MusicSource.volume - target) > 0.05f) {
			_MusicSource.volume = Mathf.Lerp (_MusicSource.volume, target, Time.deltaTime);
			yield return null;
		}
		if (target == 0) {
			_MusicSource.Stop ();
		}
	}
}
