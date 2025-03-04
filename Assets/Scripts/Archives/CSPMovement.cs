using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;


public class CSPMovement : NetworkBehaviour
{
    public float MovementSpeed = 30f;
    private CharacterController _characterController;
    private Player_Controls _playerControls;
    private bool _subscribed;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    #region Subscriptions

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        SubscribeToTimeManager(true);

        if(base.IsServer ||  base.Owner.IsLocalClient)
            _characterController.enabled = true;
        
        if (!base.Owner.IsLocalClient) return;
        _playerControls = new Player_Controls();
        _playerControls.DefaultMap.Enable();
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        SubscribeToTimeManager(false);

        if (base.Owner.IsLocalClient)
            _playerControls.DefaultMap.Disable();
    }
    
    private void SubscribeToTimeManager(bool subscribe)
    {
        if (base.TimeManager == null) return;
        if (subscribe == _subscribed) return;

        _subscribed = subscribe;

        if (subscribe)
        {
            base.TimeManager.OnTick += TimeManager_OnTick;
            base.TimeManager.OnPostTick += TimeManager_OnPostTick;
        }
        else
        {
            base.TimeManager.OnTick -= TimeManager_OnTick;
            base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
        }
    }

   
   #endregion

    private void TimeManager_OnTick()
    {
        if (IsOwner)
        {
            Reconciliate(default, false);
            GetInput(out MoveData moveData);
            Move(moveData, false);
            // Here we can add the animation, sounds and effects...
        }

        if (IsServer)
        {
            Move(default, true);
            ReconcileMoveData reconData = new ReconcileMoveData(transform.position, transform.rotation);
            Reconciliate(reconData, true);
        }
    }

    private void TimeManager_OnPostTick()
    {
        // used for rigidboddies and physic related stuff.  
    }

    private void GetInput(out MoveData moveData)
    {
        moveData = default;

        var movementInput = _playerControls.DefaultMap.Move.ReadValue<Vector2>();

        float xAxis = movementInput.x;
        float zAxis = movementInput.y;

        var IsMovementPressed = xAxis != 0 || zAxis != 0 ;

        if (!IsMovementPressed) return;
        moveData = new MoveData(xAxis, zAxis);
    }

    [Replicate]
    private void Move(MoveData moveData, bool asServer, bool replaying = false)
    {
        Vector3 move = new Vector3(moveData.XAxis, 0f, moveData.ZAxis).normalized;
        _characterController.Move(move * MovementSpeed * (float)base.TimeManager.TickDelta);
    }

    [Reconcile]
    private void Reconciliate(ReconcileMoveData recData, bool asServer)
    {
        transform.position = recData.Position;
        transform.rotation = recData.Rotation;
    }

}
