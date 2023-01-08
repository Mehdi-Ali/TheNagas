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


    private void Awake()
    {
        Instance = this;
    }

    
}
