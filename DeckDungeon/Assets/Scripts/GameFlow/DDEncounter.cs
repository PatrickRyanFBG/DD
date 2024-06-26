using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEncounter : MonoBehaviour
{
    private EEncounterPhase currentEncounterPhase = EEncounterPhase.EncounterStart;
    public EEncounterPhase CurrentPhase { get { return currentEncounterPhase; } }

    List<DDEnemyOnBoard> enemies = new List<DDEnemyOnBoard>();

    [SerializeField]
    private DDPlayer_Match player;

    public UnityEngine.Events.UnityEvent<EEncounterPhase> PhaseChanged;

    [Header("Testing")]
    [SerializeField]
    private TMPro.TextMeshProUGUI phaseDebug;

    [SerializeField]
    private TMPro.TextMeshProUGUI actionDescription;

    private DDDungeonCardEncounter currentEncounter;

    public void SetUpEncounter(DDDungeonCardEncounter encounter)
    {
        gameObject.SetActive(true);

        currentEncounter = encounter;
        currentEncounter.SpawnEnemies();

        player.ClearCards();
        player.ShuffleInDeck();
        ChangeCurrentPhase(EEncounterPhase.EncounterStart);

        actionDescription.text = "";
    }

    public void SetActionDescription(string value)
    {
        actionDescription.text = value;
    }

    public void RegisterEnemy(DDEnemyOnBoard enemy)
    {
        enemies.Add(enemy);
    }

    public void EnemyDefeated(DDEnemyOnBoard enemy)
    {
        if (enemies.Remove(enemy))
        {
            if (enemies.Count == 0)
            {
                ChangeCurrentPhase(EEncounterPhase.EncounterEnd);
            }
            else
            {
                bool allFriendly = true;
                for (int i = 0; i < enemies.Count; i++)
                {
                    if(!enemies[i].CurrentEnemy.Friendly)
                    {
                        allFriendly = false;
                        break;
                    }
                }

                if(allFriendly)
                {
                    for (int i = enemies.Count - 1; i >= 0; i--)
                    {
                        Destroy(enemies[i].gameObject);
                        enemies.RemoveAt(i);
                    }
                    ChangeCurrentPhase(EEncounterPhase.EncounterEnd);
                }
            }
        }
    }

    private void Update()
    {
        switch (currentEncounterPhase)
        {
            case EEncounterPhase.EncounterStart:
                DoEncounterStart();
                break;
            case EEncounterPhase.MonsterForecast:
                DoMonsterForecast();
                break;
            case EEncounterPhase.PlayersTurn:
                DoPlayersTurn();
                break;
            case EEncounterPhase.MonstersAct:
                break;
            case EEncounterPhase.EncounterEnd:
                DoEncounterEnd();
                break;
        }

        phaseDebug.text = currentEncounterPhase.ToString();
    }

    /*
    private void SwitchPhase(EEncounterPhase target)
    {
        switch (currentPhase)
        {
            case EEncounterPhase.EncounterStart:
                break;
            case EEncounterPhase.MonsterForecast:
                break;
            case EEncounterPhase.PlayersTurn:
                break;
            case EEncounterPhase.MonstersAct:
                break;
            case EEncounterPhase.EncounterEnd:
                break;
            default:
                break;
        }

        switch (target)
        {
            case EEncounterPhase.EncounterStart:
                break;
            case EEncounterPhase.MonsterForecast:
                break;
            case EEncounterPhase.PlayersTurn:
                break;
            case EEncounterPhase.MonstersAct:
                break;
            case EEncounterPhase.EncounterEnd:
                break;
            default:
                break;
        }

        currentPhase = target;
    }
    */

    private void DoEncounterStart()
    {
        // Do checks
        // Wait for things to come online
        ChangeCurrentPhase(EEncounterPhase.MonsterForecast);
    }

    private void DoMonsterForecast()
    {
        // Monster Priority?
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].ForecastActions(enemies.Count - i);
        }

        ChangeCurrentPhase(EEncounterPhase.PlayersTurn);
    }

    private void DoPlayersTurn()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (enemies.Count > 0)
            {
                DDEnemyOnBoard enemy = enemies[0];
                EnemyDefeated(enemy);
                Destroy(enemy.gameObject);
            }
        }
    }

    private IEnumerator DoMonstersAct()
    {
        SingletonHolder.Instance.Board.DoAllEffects();

        // Something destroyed this enemy mid action (probably bombs exploding for now).
        // So we are going backwards for now for safety
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            yield return enemies[i].DoActions();
        }

        if (currentEncounterPhase != EEncounterPhase.EncounterEnd)
        {
            ChangeCurrentPhase(EEncounterPhase.MonsterForecast);
        }
    }

    private void DoEncounterEnd()
    {
        SingletonHolder.Instance.Board.ClearAllEffects();
        gameObject.SetActive(false);
        SingletonHolder.Instance.Dungeon.EncounterCompleted(currentEncounter);
    }

    public void PlayerEndedTurn()
    {
        if(currentEncounterPhase == EEncounterPhase.PlayersTurn)
        {
            player.DiscardHand();
            ChangeCurrentPhase(EEncounterPhase.MonstersAct);
        }
    }

    private void ChangeCurrentPhase(EEncounterPhase toPhase)
    {
        currentEncounterPhase = toPhase;

        switch (currentEncounterPhase)
        {
            case EEncounterPhase.EncounterStart:
                player.SetHandSizeToDefault();
                break;
            case EEncounterPhase.MonsterForecast:
                break;
            case EEncounterPhase.PlayersTurn:
                SingletonHolder.Instance.PlayerSelector.SetToPlayerCard();
                player.ResetMomentum();
                player.DrawFullHand();
                break;
            case EEncounterPhase.MonstersAct:
                StartCoroutine(DoMonstersAct());
                break;
            case EEncounterPhase.EncounterEnd:
                break;
            default:
                break;
        }

        PhaseChanged.Invoke(currentEncounterPhase);
    }
}
