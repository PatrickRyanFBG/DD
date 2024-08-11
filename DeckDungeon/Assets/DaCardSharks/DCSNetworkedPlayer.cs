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

[System.Serializable]
public class DCSUIInformation
{
    public GameObject parent;
    public TMPro.TextMeshProUGUI health;
    public TMPro.TextMeshProUGUI streetCred;
    public TMPro.TextMeshProUGUI deckCount;
    public TMPro.TextMeshProUGUI discardCount;
}

public class DCSNetworkedPlayer : NetworkBehaviour
{
    [SerializeField]
    private DDCardBase[] startingCards;

    private List<DDCardBase> currentDeck = new List<DDCardBase>();

    [SerializeField]
    private RawImage visuals;

    private readonly IntSyncVar visualWidth = new IntSyncVar();
    private readonly IntSyncVar healthValue = new IntSyncVar();
    private readonly IntSyncVar streetCredValue = new IntSyncVar();

    private readonly SyncVar<EDCSPlayerState> playerState = new SyncVar<EDCSPlayerState>();
    public EDCSPlayerState CurrentState => playerState.Value;

    [SerializeField]
    private TMPro.TextMeshProUGUI stateText;

    [SerializeField]
    private DCSUIInformation[] infos;
    private DCSUIInformation myInfo { get { return infos[clientId.Value - 1]; } }
    private readonly SyncVar<int> clientId = new SyncVar<int>();

    private void Awake()
    {
        playerState.OnChange += PlayerState_OnChange;
        stateText.text = playerState.Value.ToString();
        clientId.OnChange += ClientId_OnChange;
        healthValue.OnChange += HealthValue_OnChange;
        streetCredValue.OnChange += StreetCredValue_OnChange;
    }

    private void StreetCredValue_OnChange(int prev, int next, bool asServer)
    {
        myInfo.streetCred.text = next + " / 100";
    }

    private void HealthValue_OnChange(int prev, int next, bool asServer)
    {
        myInfo.health.text = next + " / 50";
    }

    private void ClientId_OnChange(int prev, int next, bool asServer)
    {
        infos[next - 1].parent.SetActive(true);
        myInfo.health.text = "50 / 50";
        myInfo.streetCred.text = "0 / 100";
    }

    private void PlayerState_OnChange(EDCSPlayerState prev, EDCSPlayerState next, bool asServer)
    {
        Debug.Log("PlayerState_OnChange: " + asServer);
        //if (SingletonHolder.Instance.Player.CurrentHand.Count == 0)
        //{
        //    SingletonHolder.Instance.Player.DrawFullHand();
        //}

        stateText.text = next.ToString();
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

    [ServerRpc(RequireOwnership = false)]
    public void DealDamage(int amount)
    {
        healthValue.Value -= amount;
        streetCredValue.Value += amount;
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

            gameObject.name = "DCSNetworkedPlayer_LOCAL";
            ClientConfirmSpawned();
            DCSMatch.Instance.LocalPlayer = this;
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
        // It doesn't get updated if if it doesn't change so xdd
        clientId.Value = connection.ClientId + 1;
        healthValue.Value = 50;
        streetCredValue.Value = 0;
    }

    [TargetRpc]
    public void DrawHand(NetworkConnection conn)
    {
        SingletonHolder.Instance.Player.DrawFullHand();
        SingletonHolder.Instance.Player.UpdateDisplayNumbers();
    }

    [TargetRpc]
    public void DiscardSelectedCard(NetworkConnection conn)
    {
        SingletonHolder.Instance.Player.DiscardSelectedCard();
        SingletonHolder.Instance.Player.DrawACard();
        SingletonHolder.Instance.Player.UpdateDisplayNumbers();
    }

    [ServerRpc]
    public void UpdateDisplayNumbersServer(int deck, int discard)
    {
        UpdateDisplayNumbers(deck, discard);
    }

    [ObserversRpc]
    public void UpdateDisplayNumbers(int deck, int discard)
    {
        infos[clientId.Value - 1].deckCount.text = deck.ToString();
        infos[clientId.Value - 1].discardCount.text = discard.ToString();
    }

    public void MoveToWaiting()
    {
        playerState.Value = EDCSPlayerState.Waiting;
    }

    public void MoveToSelectAttack()
    {
        playerState.Value = EDCSPlayerState.Attacking;
    }

    public void MoveToSelectDefense(NetworkConnection conn = null)
    {
        playerState.Value = EDCSPlayerState.Defending;
    }
}
