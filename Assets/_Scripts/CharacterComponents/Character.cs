using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;


public class Character : NetworkBehaviour
{
    [SyncVar] public Player ControllingPlayer;



    public void Awake()
    {
        ControllingPlayer = Player.LocalPlayer;
        ControllingPlayer.ControlledCharacter = this;

        SettingCamera();
    }

    private void SettingCamera()
    {
        var cameraFollowController = FindObjectOfType<CameraFollowController>();
        cameraFollowController.CameraTarget = GetComponentInChildren<PlayerBase>();
    }
}
