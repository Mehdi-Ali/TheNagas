using UnityEngine;

public abstract class PlayerBaseState 
{
    public abstract void EnterState(PlayerStateManger player);

    public abstract void UpdateState(PlayerStateManger player);

    //ToCheck
    public abstract void OnCollisionEnter(PlayerStateManger player);
}
