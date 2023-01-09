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

    public bool CanStart { get; private set;}


    private void Awake()
    {
        Instance = this;
    }

    public void StartStage()
    {
        if (IsServer && CanStart)
            Debug.Log("Starting Stage....");
        
        // TODO change scene to stage01level01

    }

    public void UpdateCanStartStatus()
    {
        CanStart = true;

        if (Players.Count == 1)
        {
            UpdateUI();
            return;
        }

        foreach (var player in Players)
        {
            if (player.IsReady)
                continue;

            CanStart = false;
            break;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (IsClient)
            ViewsManager.Instance.UpdateCurrentView();
    }
    
}
