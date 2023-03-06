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

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        SceneManager.OnLoadEnd += SceneManagerOnLoadEnd;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        SceneManager.OnLoadEnd -= SceneManagerOnLoadEnd;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        StagePlayers.AddRange(GameManager.Instance.Players);
        StartTheStage();
    }

    [Server]
    public void StartTheStage()
    {
        Invoke(nameof(SpawningCharacters), 1f);
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

        if(IsClient)
        {
            if (Level > 1)
                Player.LocalPlayer.ControlledCharacter.ControllingPlayerStateManger.OnLoadStage();
        }

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
        ReSetCharactersTransform(StagePlayers);

        var characters = StagePlayers.Select(player => player.ControlledCharacter.NetworkObject);
        var playersAndCharacter = StagePlayers.Select(player => player.NetworkObject).Concat(characters);

        var additionalMovedNetworkObjects = new NetworkObject[] {this.NetworkObject};
        SceneLoadData sld = new(sceneName)
        {
            MovedNetworkObjects = playersAndCharacter.Concat(additionalMovedNetworkObjects).ToArray(),

            // MovedNetworkObjects = GetPlayersArray(StagePlayers).Concat(AdditionalMovedNetworkObjects).ToArray(),
            ReplaceScenes = ReplaceOption.All
        };

        SceneManager.LoadGlobalScenes(sld);
    }

    private void ReSetCharactersTransform(SyncList<Player> stagePlayers)
    {
        foreach(var player in stagePlayers)
        {
            Debug.Log("ReSetCharacterTransform");

            var playerBase = player.ControlledCharacter.GetComponentInChildren<PlayerBase>();
            var playerTrans = playerBase.transform;

            // 0.048f 
            playerTrans.position = new(0f, 1f, 0f);
            playerTrans.rotation = Quaternion.identity;
        }
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

    private void GettingEnemies()
    {
        Enemies.Clear();
        Enemies.AddRange(FindObjectsOfType<EnemyBase>().ToList());
    }
}
