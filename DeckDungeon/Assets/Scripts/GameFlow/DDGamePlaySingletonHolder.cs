using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDGamePlaySingletonHolder : MonoBehaviour
{
    private static DDGamePlaySingletonHolder instance;
    public static DDGamePlaySingletonHolder Instance => instance;

    public Camera MainCamera;
    public DDDungeon Dungeon;
    public DDEncounter Encounter;
    public DDBoard Board;
    //public DDCardLibrary CardLibrary;
    public DDEnemyLibrary EnemyLibrary;
    public DDPlayerMatch Player;
    public DDPlayerSelector PlayerSelector;
    public DDCanvasShowDeckArea ShowDeckArea;

    private void Awake()
    {
        instance = this;
    }
}
