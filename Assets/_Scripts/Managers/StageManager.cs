using System.Linq;
using System.Net.Sockets;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using FishNet.Connection;
using System.Collections.Generic;
using System;
using FishNet.Managing.Scened;
using System.Collections;

public sealed class StageManager : NetworkBehaviour
{
    public static StageManager Instance  {get; private set; }

    [SyncObject]
    public readonly SyncList<Player> StagePlayers = new();

    [field: SyncVar]
    public int Level { get; private set; }

    [SyncObject]
    public readonly SyncList<EnemyBase> Enemies = new();

    public void Awake()
    {
        Instance = this;
        Level = 1;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        StagePlayers.AddRange(GameManager.Instance.Players);
        StartTheStage();

        SceneManager.OnLoadEnd += SceneManagerOnLoadEnd;
    }

    [Server]
    public void StartTheStage()
    {
        Invoke(nameof(SpawningCharacters), 1.0f);
    }

    private void SpawningCharacters()
    {
        foreach (var player in StagePlayers)
        {
            player.SpawnCharacter();
        }
    }

    [Server]
    public void EndTheStage()
    {
        foreach (var player in StagePlayers)
        {
            player.DesspawnCharacter();
        }
    }

    private void SceneManagerOnLoadEnd(SceneLoadEndEventArgs obj)
    {
        Debug.Log("SceneManagerOnLoadEnd " + Level);

        GettingEnemies();
    }

    private void GettingEnemies()
    {
        Enemies.Clear();
        Enemies.AddRange(FindObjectsOfType<EnemyBase>().ToList());
    }

    [Server]
    public void ServerNextLevel()
    {
        if (Enemies.Count == 0)
        {
            Level++;
            Debug.Log("Stage01Level0" + Level.ToString());
            if (Level <= 10)
                LoadNextLevel("Stage01Level0" + Level.ToString());
            else
                Debug.Log("Finished");
        }
    }

    private void LoadNextLevel(string sceneName)
    {
        var AdditionalMovedNetworkObjects = new NetworkObject[] {this.NetworkObject};
        SceneLoadData sld = new(sceneName)
        {
            MovedNetworkObjects = GetPlayersArray(StagePlayers).Concat(AdditionalMovedNetworkObjects).ToArray(),
            ReplaceScenes = ReplaceOption.All
        };

        SceneManager.LoadGlobalScenes(sld);
    }

    private NetworkObject[] GetPlayersArray(SyncList<Player> players)
    {
        NetworkObject[] netObj = new NetworkObject[players.Count];

        for (int i = 0; i < players.Count; i++)
        {
            netObj[i] = players[i].NetworkObject;
            netObj[i] = players[i].ControlledCharacter.NetworkObject;
        }

        return netObj;
    }
}
