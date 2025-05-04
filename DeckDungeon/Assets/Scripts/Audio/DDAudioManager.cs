using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDAudioManager : MonoBehaviour
{
    [SerializeField] private DDAudioPlayer playerPrefab;
    [SerializeField] private int poolInit = 25;

    [SerializeField] private DDAudioClip backgroundMusic;
    
    private DDAudioPlayer backgroundMusicPlayer;
    
    private Queue<DDAudioPlayer> unusedPlayers = new Queue<DDAudioPlayer>();
    private List<DDAudioPlayer> usedPlayers = new List<DDAudioPlayer>();

    private void Awake()
    {
        for (int i = 0; i < poolInit; i++)
        {
            DDAudioPlayer p = Instantiate(playerPrefab, transform);
            unusedPlayers.Enqueue(p);
        }
        
        backgroundMusicPlayer = Instantiate(playerPrefab, transform);
        backgroundMusicPlayer.PlayAudio(backgroundMusic);
    }

    public DDAudioPlayer PlayNow(DDAudioClip audioClip)
    {
        DDAudioPlayer player;
        if (!unusedPlayers.TryDequeue(out player))
        { 
            player = Instantiate(playerPrefab, transform);
        }
        usedPlayers.Add(player);
        
        player.PlayAudio(audioClip);
        
        return player;
    }

    public void PlayLater(DDAudioClip audioClip, float waitTime)
    {
        StartCoroutine(WaitingToPlay(audioClip, waitTime));
    }

    private IEnumerator WaitingToPlay(DDAudioClip audioClip, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        PlayNow(audioClip);
    }

    public void ReleaseAudioPlayer(DDAudioPlayer p)
    {
        usedPlayers.Remove(p);
        unusedPlayers.Enqueue(p);
    }
}