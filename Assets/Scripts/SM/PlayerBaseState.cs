using UnityEngine;
using FishNet.Object;

public abstract class PlayerBaseState : NetworkBehaviour 
{

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

}
