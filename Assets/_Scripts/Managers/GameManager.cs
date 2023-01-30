using System.Net.Sockets;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using FishNet.Connection;
using System.Collections.Generic;
using FishNet.Managing.Scened;
using System.Linq;
using System;

public sealed class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SyncObject(ReadPermissions = ReadPermission.Observers)]
    public readonly SyncList<Player> Players = new();
    public string[] PlayersNickNames;
    public bool[] PlayersReadyState;
    public string NextSceneToLoad;

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
        NextSceneToLoad = "Stage01Level01" ;

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

    #region UI
    
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
    
    #endregion


    #region SceneManager

    public void ServerStartStage()
    {
        // if (IsServer && CanStart)
        //     ScenesManager.Instance.GlobalLoad(NextSceneToLoad);

         if (!IsServer || !CanStart)
            return;


        SceneLoadData sld = new(NextSceneToLoad)
        {
            MovedNetworkObjects = GetPlayersArray(Players),
            ReplaceScenes = ReplaceOption.All
        };
        base.SceneManager.LoadGlobalScenes(sld);
    }

    private NetworkObject[] GetPlayersArray(SyncList<Player> players)
    {
        NetworkObject[] netObj = new NetworkObject[players.Count] ;

        for (int i = 0; i < players.Count; i++)
        {
            netObj[i] = players[i].NetworkObject;
        }

        return netObj;
    }

    #endregion
    
}
