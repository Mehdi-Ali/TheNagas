using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public sealed class GameManager : NetworkBehaviour
{
    public static GameManager Instance  {get; private set; }

    [SyncObject]
    public readonly SyncList<Player> Players = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        
    }
}
