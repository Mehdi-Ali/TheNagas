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

    public int Level;

    public void Awake()
    {
        Instance = this;
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


}
