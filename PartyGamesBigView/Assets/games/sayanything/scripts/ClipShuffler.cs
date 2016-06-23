using UnityEngine;
using System.Collections;

public class ClipShuffler {
    int _currentClip = 0;
    int _numClips = 0;
    public AudioClip[] Clips;

    public ClipShuffler(AudioClip[] clips)
    {
        Clips = clips;
        _numClips = clips.Length;

        ShuffleClips();
    }

    public AudioClip GetNext()
    {
        if (++_currentClip >= _numClips)
        {
            ShuffleClips();
            _currentClip = 0;
        }

        return Clips[_currentClip];
    }

    void ShuffleClips()
    {
        ArrayHelper.Shuffle<AudioClip>(Clips);
    }
}
