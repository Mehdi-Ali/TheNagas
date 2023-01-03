using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Player : NetworkBehaviour
{
    
    public static Player Instance {get; private set;}

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

        Instance = this;

        ViewsManager.Instance.Initialize();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.Instance.Players.Remove(this);
    }
}
