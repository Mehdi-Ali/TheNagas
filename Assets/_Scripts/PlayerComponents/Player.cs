using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Player : NetworkBehaviour
{
    
    public static Player LocalPlayer {get; private set;}

    [field: SyncVar] public int PlayerName
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

    [SyncVar] public Character ControlledCharacter;
    public bool AAAA;



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

        ViewsManager.Instance.Initialize();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.Instance.Players.Remove(this);
    }


    [ServerRpc]
    public void ServerSpawnCharacter()
    {
        if (ControlledCharacter != null)
            return;

        var characterPrefab = Addressables.LoadAssetAsync<GameObject>("KnightCharacter").WaitForCompletion();

        var characterInstance = Instantiate(characterPrefab);

        Spawn(characterInstance, Owner);

    }
}
