using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[System.Serializable]
public class DDAudioClip
{
    [SerializeField] private AudioClip[] clips;

    [SerializeField] private AudioMixerGroup mixerGroup;
    public AudioMixerGroup MixerGroup => mixerGroup;

    [SerializeField] private float volume = 0.5f;
    public float Volume => volume;

    [SerializeField] private bool looping = false;
    public bool Looping => looping;
    
    public AudioClip GetClip()
    {
        if (clips.Length == 0)
        {
            return null;
        }
        else if (clips.Length == 1)
        {
            return clips[0];
        }
        else
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }

    public void PlayNow()
    {
        DDGlobalManager.Instance.AudioManager.PlayNow(this);
    }
}
