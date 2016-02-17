using UnityEngine;
using System.Collections;
using PPlatform.Helper;

public class AudioManager : SceneSingleton<AudioManager>
{
    public AudioClip _OnUserJoin;

    public AudioClip _BackgroundMusic;

    public AudioSource _SoundSource;
    public AudioSource _MusicSource;


	// Use this for initialization
	void Start () {

        PlayMusic(_BackgroundMusic);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void OnUserJoin()
    {
        PlaySound(_OnUserJoin);

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
        }
    }
}
