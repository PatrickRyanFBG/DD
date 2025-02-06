using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDGamePlaySingletonHolder : MonoBehaviour
{
    private static DDGamePlaySingletonHolder instance;
    public static DDGamePlaySingletonHolder Instance => instance;

    public DDDungeon Dungeon;
    public DDEncounter Encounter;
    public DDBoard Board;
    //public DDCardLibrary CardLibrary;
    public DDEnemyLibrary EnemyLibrary;
    public DDPlayer_Match Player;
    public DDPlayerSelector PlayerSelector;
    public DDShowDeckArea ShowDeckArea;

    private void Awake()
    {
        instance = this;
    }
}
