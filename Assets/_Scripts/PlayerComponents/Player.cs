using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Player : NetworkBehaviour
{
    // TODO Separate the logic to Player, LobbyPlayer, GamePlayer...

    public static Player LocalPlayer {get; private set;}

    [field: SyncVar] public string PlayerNickName
    {
        get;

        [ServerRpc]
        private set;
    }

    // try remove the field: should be okay.
    [field: SerializeField] [field: SyncVar] public int Score
    {
        get;

        [ServerRpc]
        private set;
    }


    [SyncVar(OnChange = nameof(OnIsReadyChanged))]
    public bool IsReady;
    private void OnIsReadyChanged(bool prev, bool next, bool asServer)
    {
        GameManager.Instance.UpdateCanStartStatus();
    }


    private string _characterChosen;
    [SyncVar] public Character ControlledCharacter;

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameManager.Instance.Players.Add(this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
            return;

        LocalPlayer = this;
        ServerSetPlayerName(SceneDataTransferManager.Instance.PlayerNickName);
        ViewsManager.Instance.Initialize();
        Debug.Log(PlayerNickName);
    }

    // ! may be unnecessary because the set of name is already serverRpc 
    [ServerRpc]
    private void ServerSetPlayerName(string nickname)
    {
        PlayerNickName = nickname;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GameManager.Instance.Players.Remove(this);
    }



    [ServerRpc]
    public void ServerSpawnCharacter()
    {
        // TODO set this when the player chose a character
        _characterChosen = "KnightCharacter";

        if (ControlledCharacter != null)
            return;

        var characterPrefab = Addressables.LoadAssetAsync<GameObject>(_characterChosen).WaitForCompletion();

        var characterInstance = Instantiate(characterPrefab);

        Spawn(characterInstance, Owner);

    }

    [ServerRpc]
    public void ServerDesSpawnCharacter()
    {
        if (ControlledCharacter != null && ControlledCharacter.IsSpawned)
            ControlledCharacter.Despawn();
    }
}
