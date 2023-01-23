using System.Net.Sockets;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using FishNet.Connection;
using System.Collections.Generic;

public sealed class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SyncObject(ReadPermissions = ReadPermission.Observers)]
    public readonly SyncList<Player> Players = new();
    public string[] PlayersNickNames;
    public bool[] PlayersReadyState;

    [field: SyncVar]
    public bool CanStart { get; private set;}

    [field: SyncVar]
    public bool LeaderAssigned
    {
        get;

        [ServerRpc(RunLocally = true,RequireOwnership = false)]
        set;
    }


    public void Awake()
    {
        Instance = this;
        CanStart = false;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        LeaderAssigned = false;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayersNickNames = new string[4];
        PlayersReadyState = new bool[4];
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
        UpdatePlayerNamesAndStatus();
        ObserversUpdateUI();
    }

    private void UpdatePlayerNamesAndStatus()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Player player = Players[i];
            var name = player.PlayerNickName;
            var isReady = player.IsReady;

            TargetUpdatePlayerStatus(i, name, isReady);
        }
    }

    [ObserversRpc(RunLocally = true)]
    public void ObserversUpdateUI()
    {
        ViewsManager.Instance.UpdateCurrentView();
    }

    [ObserversRpc(RunLocally = true)]
    private void TargetUpdatePlayerStatus(int index, string nickName, bool isReady)
    {
        PlayersNickNames[index] = nickName;
        PlayersReadyState[index] = isReady;
    }
    
}
