using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public enum EDCSPlayerState
{
    Waiting,
    Attacking,
    Defending,
    Looting,
    Dead
}

public class DCSNetworkedPlayer : NetworkBehaviour
{
    [SerializeField]
    private DDCardBase[] startingCards;

    private List<DDCardBase> currentDeck = new List<DDCardBase>();

    [SerializeField]
    private RawImage visuals;

    private readonly IntSyncVar visualWidth = new IntSyncVar();

    private readonly SyncVar<EDCSPlayerState> playerState = new SyncVar<EDCSPlayerState>(new SyncTypeSettings(ReadPermission.ExcludeOwner));
    
    private void Awake()
    {
        playerState.OnChange += PlayerState_OnChange;
    }

    private void PlayerState_OnChange(EDCSPlayerState prev, EDCSPlayerState next, bool asServer)
    {
        Debug.Log("PlayerState_OnChange: " + asServer);
        switch (next)
        {
            case EDCSPlayerState.Waiting:
                break;
            case EDCSPlayerState.Attacking:
                break;
            case EDCSPlayerState.Defending:
                break;
            case EDCSPlayerState.Looting:
                break;
            case EDCSPlayerState.Dead:
                break;
            default:
                break;
        }
    }

    private void VisualWidth_OnChange(int prev, int next, bool asServer)
    {
        Rect uvRec = visuals.uvRect;
        uvRec.width = next;
        visuals.uvRect = uvRec;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Is Me: " + base.IsOwner);
        visualWidth.OnChange += VisualWidth_OnChange;

        if (base.IsOwner)
        {
            currentDeck.AddRange(startingCards);
            SingletonHolder.Instance.Player.SetHandSizeToDefault();
            SingletonHolder.Instance.Player.ShuffleInDeck(currentDeck);
            //DCSMatch.Instance.UpdateDeckDiscardNumbers(currentDeck.Count, 0);

            gameObject.name = "DCSNetworkedPlayer_LOCAL";
            ClientConfirmSpawned();
        }
        else
        {
            gameObject.name = "DCSNetworkedPlayer_REMOTE";
        }
    }
    
    [ServerRpc]
    public void ClientConfirmSpawned(NetworkConnection connection = null)
    {
        base.OnSpawnServer(connection);
        Debug.Log("OnSpawnServer: " + connection.ClientId + " ObjectId: " + base.ObjectId);
        visualWidth.Value = connection.ClientId == 0 ? -1 : 1;
    }

    [TargetRpc]
    public void DrawHand(NetworkConnection conn)
    {
        SingletonHolder.Instance.Player.DrawFullHand();
    }
    
    public void MoveToWaiting()
    {
        playerState.Value = EDCSPlayerState.Waiting;
    }

    public void MoveToSelectAttack()
    {
        playerState.Value = EDCSPlayerState.Attacking;
    }

    public void MoveToSelectDefense()
    {
        playerState.Value = EDCSPlayerState.Defending;
    }
}
