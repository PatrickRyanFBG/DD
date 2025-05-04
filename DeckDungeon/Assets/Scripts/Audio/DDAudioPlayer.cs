using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private bool audioPaused = false;
    
    public void PlayAudio(DDAudioClip clip)
    {
        audioSource.clip = clip.GetClip();
        audioSource.outputAudioMixerGroup = clip.MixerGroup;
        audioSource.volume = clip.Volume;
        audioSource.loop = clip.Looping;
        
        gameObject.SetActive(true);
        
        audioSource.Play();
    }

    public void Paused()
    {
        if (audioSource.isPlaying)
        {
            audioPaused = true;
            audioSource.Pause();
        }
    }

    public void Unpaused()
    {
        if (audioPaused)
        {
            audioSource.UnPause();
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying && !audioPaused)
        {
            DDGlobalManager.Instance.AudioManager.ReleaseAudioPlayer(this);
            gameObject.SetActive(false);
        }
    }
}
