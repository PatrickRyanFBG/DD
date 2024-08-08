using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public enum EDCSMatchState
{
    Loading,
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

    public TMPro.TextMeshProUGUI[] playerDeckCounts;
    public TMPro.TextMeshProUGUI[] playerDiscardCounts;

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
        ClientReady();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientReady(NetworkConnection connection = null)
    {
        DCSPlayerInfo info = new DCSPlayerInfo();
        info.conn = connection;
        info.player = connection.FirstObject.GetComponent<DCSNetworkedPlayer>();
        connectedPlayers.Add(info);
        Debug.Log("Server: Client Ready " + connection.ClientId);

        if (connectedPlayers.Count == 2)
        {
            AllClientsConnected();
        }
    }

    public void AllClientsConnected()
    {
        foreach (var item in connectedPlayers)
        {
            item.player.DrawHand(item.conn);
        }

        connectedPlayers[0].player.MoveToSelectAttack();
    }

    [ObserversRpc]
    public void UpdateDeckDiscardNumbers(int deck, int discard, NetworkConnection conn = null)
    {
        if(conn.ClientId < 0 || conn.ClientId > playerDeckCounts.Length)
        {
            return;
        }
        playerDeckCounts[conn.ClientId].text = deck.ToString();
        playerDiscardCounts[conn.ClientId].text = discard.ToString();
    }
}
