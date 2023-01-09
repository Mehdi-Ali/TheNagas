using System.Net.Sockets;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using FishNet.Connection;


public sealed class StageManager : NetworkBehaviour
{
    public static StageManager Instance  {get; private set; }

    [SyncObject]
    public readonly SyncList<Player> StagePlayers = new();

    private void Awake()
    {
        Instance = this;
        StartTheStage();
    }

    [Server]
    public void StartTheStage()
    {
        foreach(var player in StagePlayers)
        {
            player.ServerSpawnCharacter();
        }
    }

    [Server]
    public void EndTheStage()
    {
        foreach (var player in StagePlayers)
        {
            player.ServerDesSpawnCharacter();
        }
    }


}
