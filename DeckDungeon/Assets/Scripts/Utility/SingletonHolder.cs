using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonHolder : MonoBehaviour
{
    private static SingletonHolder instance;
    public static SingletonHolder Instance { get { return instance; } }

    public DDDungeon Dungeon;
    public DDEncounter Encounter;
    public DDBoard Board;
    public DDCardLibrary CardLibrary;
    public DDEnemyLibrary EnemyLibrary;
    public DDPlayer_Match Player;
    public DDPlayerSelector PlayerSelector;

    private void Awake()
    {
        instance = this;
    }
}
