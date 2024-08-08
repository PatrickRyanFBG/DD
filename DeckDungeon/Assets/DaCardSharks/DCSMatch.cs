using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public enum EDCSMatchState
{
    Loading,
    Selecting,
    Fighting,
    Looting,
    Ending
}

public class DCSPlayerInfo
{
    public NetworkConnection conn;
    public DCSNetworkedPlayer player;
}

public class DCSMatch : NetworkBehaviour
{
    public static DCSMatch Instance;

    private List<DCSPlayerInfo> connectedPlayers = new List<DCSPlayerInfo>();

    private readonly SyncVar<EDCSMatchState> matchState = new SyncVar<EDCSMatchState>();

    public DCSNetworkedPlayer LocalPlayer;

    [SerializeField]
    private DDCardInHand[] cardsSelected;

    [SerializeField]
    private List<DDCardBase> allCards;

    private int lastPlayerAttacking = 0;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        base.OnStopServer();
        Debug.Log("I'm Server.");
        matchState.Value = EDCSMatchState.Loading;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("I'm Client.");
        //enabled = false;
        //ClientReady();
    }

    public void ClientReady(NetworkConnection connection = null)
    {
        DCSPlayerInfo info = new DCSPlayerInfo();
        info.conn = connection;
        info.player = connection.FirstObject.GetComponent<DCSNetworkedPlayer>();
        connectedPlayers.Add(info);
        Debug.Log("Server: Client Ready " + connection.ClientId);

        if (connectedPlayers.Count == 2)
        {
            StartCoroutine(AllClientsConnected());
        }
    }

    public IEnumerator AllClientsConnected()
    {
        //yield return new WaitForSeconds(1f);

        while(!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        foreach (var item in connectedPlayers)
        {
            item.player.DrawHand(item.conn);
        }

        matchState.Value = EDCSMatchState.Selecting;

        connectedPlayers[0].player.MoveToSelectAttack();
        connectedPlayers[1].player.MoveToWaiting();
    }

    [ServerRpc(RequireOwnership = false)]
    public void CardSelectedServer(int index, NetworkConnection conn = null)
    {
        CardSelected(conn.ClientId, index);

        if (connectedPlayers[conn.ClientId].player.CurrentState == EDCSPlayerState.Attacking)
        {
            connectedPlayers[(conn.ClientId + 1) % 2].player.MoveToSelectDefense();
        }
        else if (connectedPlayers[conn.ClientId].player.CurrentState == EDCSPlayerState.Defending)
        {
            // HERE WE SELECTED A DEFENSE CARD GOTTA DO BATTLE
            matchState.Value = EDCSMatchState.Fighting;
            DoBattle();
            // connectedPlayers[conn.ClientId].player.MoveToSelectAttack();
        }

        connectedPlayers[conn.ClientId].player.MoveToWaiting();
    }

    [ObserversRpc]
    public void CardSelected(int playerIndex, int cardIndex)
    {
        cardsSelected[playerIndex].SetUpCard(allCards[cardIndex]);
    }

    public int GetCardIndex(DDCardBase card)
    {
        return allCards.IndexOf(card);
    }

    [ObserversRpc]
    public void DoBattle()
    {
        StartCoroutine(DoBattleOverTime());
    }

    private IEnumerator DoBattleOverTime()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < cardsSelected.Length; i++)
        {
            cardsSelected[i].gameObject.SetActive(false);
        }

        foreach (var item in connectedPlayers)
        {
            item.player.DiscardSelectedCard(item.conn);
        }

        yield return new WaitForSeconds(1f);

        if(IsServerStarted)
        {
            matchState.Value = EDCSMatchState.Selecting;

            lastPlayerAttacking = (lastPlayerAttacking + 1) % 2;

            connectedPlayers[lastPlayerAttacking].player.MoveToSelectAttack();
            connectedPlayers[(lastPlayerAttacking + 1) % 2].player.MoveToWaiting();
        }
    }
}
