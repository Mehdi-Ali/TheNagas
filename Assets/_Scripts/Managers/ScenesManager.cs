// using System;
// using FishNet;
// using FishNet.Managing.Logging;
// using FishNet.Managing.Scened;
// using FishNet.Object;
// using FishNet.Object.Synchronizing;
// using UnityEngine;

// public sealed class ScenesManager : NetworkBehaviour
// {
//     public static ScenesManager Instance {get; private set;}

//     [SerializeField]
//     public NetworkObject[] MovedNetworkObjects;


//     public override void OnStartClient()
//     {
//         base.OnStartClient();

//         base.SceneManager.OnLoadEnd += OnLoadEnd;
//     }

//     public override void OnStopClient()
//     {
//         base.OnStopClient();
//         base.SceneManager.OnLoadEnd -= OnLoadEnd;

//     }

//     public void GlobalLoad(string sceneName)
//     {
//         if (!IsServer)
//             return;

//         SceneLoadData sld = new(sceneName)
//         {
//             MovedNetworkObjects = GetPlayersArray(Players),
//             ReplaceScenes = ReplaceOption.All
//         };
//         base.SceneManager.LoadGlobalScenes(sld);
//     }


//     private NetworkObject[] GetPlayersArray(SyncList<Player> players)
//     {
//         NetworkObject[] netObj = new NetworkObject[players.Count];

//         for (int i = 0; i < players.Count; i++)
//         {
//             netObj[i] = players[i].NetworkObject;
//         }

//         return netObj;
//     }


//     private void OnLoadEnd(SceneLoadEndEventArgs obj)
//     {
//         if(StageManager.Instance == null)
//             return;
        
//         StageManager.Instance.StagePlayers.AddRange(Players);
//         StageManager.Instance.StartTheStage();
//     }
// }
