using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Player : NetworkBehaviour
{
    // TODO Separate the logic to Player, LobbyPlayer, GamePlayer...

    public static Player LocalPlayer {get; private set;}

    [field: SerializeField]
    [field: SyncVar(ReadPermissions = ReadPermission.Observers)]
    public string PlayerNickName
    {
        get;

        [ServerRpc(RunLocally = true)]
        private set;
    }

    [field: SerializeField] [field: SyncVar]
    public bool IsReady
    {
        get;

        [ServerRpc(RunLocally = true)]
        private set;
    }

    private string _characterChosenPath;
    [SyncVar] public Character ControlledCharacter;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (GameManager.Instance.Players.Count >= 4 )
        {
            Debug.Log("The Lobby is already full");
            return;
        }
        
        GameManager.Instance.Players.Add(this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
            return;

        LocalPlayer = this;
        ViewsManager.Instance.Initialize();

        SetNickname(SceneDataTransferManager.Instance.PlayerNickName);

        if (!GameManager.Instance.LeaderAssigned)
        {
            SetIsReadyStatus(true);
            GameManager.Instance.LeaderAssigned = true;
        }
        else
            SetIsReadyStatus(false);   
            
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GameManager.Instance.Players.Remove(this);
    }

    [Server]
    public void SpawnCharacter()
    {
        // TODO set this when the player chose a character
        _characterChosenPath = "KnightCharacter";

        if (ControlledCharacter != null)
            return;

        var characterPrefab = Addressables.LoadAssetAsync<GameObject>(_characterChosenPath).WaitForCompletion();

        var characterInstance = Instantiate(characterPrefab);

        Spawn(characterInstance, Owner);
    }

    [Server]
    public void DesspawnCharacter()
    {
        if (ControlledCharacter != null && ControlledCharacter.IsSpawned)
            ControlledCharacter.Despawn();
    }

    public void SetNickname(string nickName)
    {
        PlayerNickName = nickName;
        GameManager.Instance.ServerUpdateUI();
    }

    public void SetIsReadyStatus(bool isReady)
    {
        this.IsReady = isReady;

        GameManager.Instance.ServerUpdateCanStartStatus();
        GameManager.Instance.ServerUpdateUI();
    }
}
