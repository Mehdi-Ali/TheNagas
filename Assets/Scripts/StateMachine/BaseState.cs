using FishNet.Object;

public abstract class BaseState : NetworkBehaviour 
{

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

}
