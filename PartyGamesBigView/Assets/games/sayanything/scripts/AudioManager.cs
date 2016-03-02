using UnityEngine;
using System.Collections;
using PPlatform.Helper;

public class AudioManager : SceneSingleton<AudioManager>
{
    public AudioClip _OnUserJoin;
    public AudioClip _BackgroundMusic;

    public AudioSource _SoundSource;
    public AudioSource _MusicSource;

	void Start (){
		PlayMusic(_BackgroundMusic);
	}

	// Update is called once per frame
	void Update () {
	
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
