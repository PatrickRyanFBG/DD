using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEncounter : MonoBehaviour
{
    private EEncounterPhase currentEncounterPhase = EEncounterPhase.EncounterStart;
    public EEncounterPhase CurrentPhase => currentEncounterPhase;

    private List<DDEnemyOnBoard> enemies = new List<DDEnemyOnBoard>();
    public List<DDEnemyOnBoard> AllEnemies => enemies;

    [SerializeField] private DDPlayer_Match player;

    public UnityEngine.Events.UnityEvent<EEncounterPhase> PhaseChanged;

    [Header("Testing")] [SerializeField] private TMPro.TextMeshProUGUI phaseDebug;

    private DDDungeonCardEncounter currentEncounter;
    
    private bool playersTurnEnding;

    public void SetUpEncounter(DDDungeonCardEncounter encounter)
    {
        gameObject.SetActive(true);

        currentEncounter = encounter;
        currentEncounter.SpawnEnemies();

        player.EncounterStarted();
        player.ShuffleInDeck();
        ChangeCurrentPhase(EEncounterPhase.EncounterStart);
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
                    if (!enemies[i].CurrentEnemy.Friendly)
                    {
                        allFriendly = false;
                        break;
                    }
                }

                if (allFriendly)
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

    private IEnumerator DoPlayersEndTurn()
    {
        playersTurnEnding = true;
        yield return player.EndOfTurn();
        yield return player.DiscardHand();
        ChangeCurrentPhase(EEncounterPhase.MonstersAct);
    }

    private IEnumerator DoMonstersAct()
    {
        DDGamePlaySingletonHolder.Instance.Board.DoAllEffects();

        yield return DoMonsterAffixes();
        
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

    private IEnumerator DoMonsterAffixes()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            yield return enemies[i].DoAffixes();
        }
    }

    private void DoEncounterEnd()
    {
        DDGamePlaySingletonHolder.Instance.Board.ClearAllEffects();
        gameObject.SetActive(false);
        DDGamePlaySingletonHolder.Instance.Dungeon.EncounterCompleted(currentEncounter);
    }

    public void PlayerEndedTurn()
    {
        if (currentEncounterPhase == EEncounterPhase.PlayersTurn && !playersTurnEnding)
        {
            ChangeCurrentPhase(EEncounterPhase.PlayersEndTurn);
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
                DDGamePlaySingletonHolder.Instance.PlayerSelector.SetToPlayerCard();
                player.ResetMomentum();
                player.DrawFullHand();
                break;
            case EEncounterPhase.PlayersEndTurn:
                StartCoroutine(DoPlayersEndTurn());
                break;
            case EEncounterPhase.MonstersAct:
                playersTurnEnding = false;
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