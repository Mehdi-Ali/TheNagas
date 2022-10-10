using UnityEngine;

public abstract class BaseState : MonoBehaviour 
{

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

}
