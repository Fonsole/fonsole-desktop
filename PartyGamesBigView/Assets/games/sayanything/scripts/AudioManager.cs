using UnityEngine;
using System.Collections;
using PPlatform.Helper;

public class AudioManager : SceneSingleton<AudioManager>
{
    public AudioClip _OnUserJoin;
    public AudioClip _BackgroundMusic;
    public AudioClip[] _OnQuestionSelected;

    public AudioSource _SoundSource;
    public AudioSource _MusicSource;

    private bool _MuteToggle;

	void Start (){
        AudioListener.volume = 0.50f;
		PlayMusic(_BackgroundMusic);
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
        if (clip != null)
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
