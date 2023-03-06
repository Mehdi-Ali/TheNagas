using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;


public class Character : NetworkBehaviour
{
    [SyncVar] public Player ControllingPlayer;
    public PlayerBase ControllingPlayerBase;
    public PlayerStateManger ControllingPlayerStateManger;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        ControllingPlayer = Player.LocalPlayer;
        ControllingPlayer.ControlledCharacter = this;
        ControllingPlayerBase = GetComponentInChildren<PlayerBase>();
        ControllingPlayerStateManger = GetComponentInChildren<PlayerStateManger>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        SettingCamera();
    }

    public void SettingCamera()
    {
        if (!base.IsOwner)
            return;

        var cameraFollowController = FindObjectOfType<CameraFollowController>();
        cameraFollowController.CameraTarget = GetComponentInChildren<PlayerBase>();
    }
}
