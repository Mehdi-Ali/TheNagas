using System.Net.Sockets;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using FishNet.Connection;

public sealed class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SyncObject]
    public readonly SyncList<Player> Players = new();

    [field: SyncVar]
    public bool CanStart { get; private set;}


    private void Awake()
    {
        Instance = this;
        CanStart = false;
    }

    public void ServerStartStage()
    {
        if (IsServer && CanStart)
            Debug.Log("Starting Stage....");
        
        // TODO change scene to stage01level01

    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerUpdateCanStartStatus()
    {
        CanStart = true;

        foreach (var player in Players)
        {
            if (player.IsReady)
                continue;

            CanStart = false;
            break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerUpdateUI()
    {
        ObserverUpdateUI();
        Debug.Log("ServerUpdated.");

    }

    [ObserversRpc(RunLocally = true)]
    public void ObserverUpdateUI()
    {
        ViewsManager.Instance.UpdateCurrentView();
        Debug.Log("ObserversUpdated.");
    }

    // public void Update()
    // {
    //     // TODO find a solution for this 
    //     //if (IsServer)
    //         ObserverUpdateUI();
    // }
    
}
