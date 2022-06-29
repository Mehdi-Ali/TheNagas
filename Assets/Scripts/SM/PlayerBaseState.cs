using UnityEngine;
using FishNet.Object;

public abstract class PlayerBaseState : NetworkBehaviour 
{

    public abstract void EnterState(PlayerStateManger player);

    public abstract void UpdateState(PlayerStateManger player);

    public abstract void ExitState(PlayerStateManger player);

}
