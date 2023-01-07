using FishNet.Object;
using FishNet.Object.Synchronizing;

public class Character : NetworkBehaviour
{
    [SyncVar] public Player ControllingPlayer; 


    void Awake()
    {
        ControllingPlayer = Player.LocalPlayer;
        ControllingPlayer.ControlledCharacter = this;
    }
}
