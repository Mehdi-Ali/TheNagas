using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;


public class CSPMovement : NetworkBehaviour
{

    #region structData

    public struct MoveData
    {
        public float Horizontal;
        public float Vertical;

        public MoveData(float horizontal, float vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }
    }

    public struct ReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public ReconcileData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }

    #endregion

    public float MovementSpeed = 30f;
    private CharacterController _characterController;
    private Player_Controls _playerControls;
    private bool _subscribed;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerControls = new Player_Controls();
    }

    #region Subscriptions

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

    private void OnDestroy()
    {
        SubscribeToTimeManager(false);
        _playerControls.DefaultMap.Disable();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SubscribeToTimeManager(true);
        _characterController.enabled = (base.IsServer || base.IsOwner);
        _playerControls.DefaultMap.Enable();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        SubscribeToTimeManager(true);
    }
   
   #endregion

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconciliate(default, false);
            GetInput(out MoveData moveData);
            Move(moveData, false);
        }

        if (base.IsServer)
        {
            Move(default, true);
            ReconcileData reconData = new ReconcileData(transform.position, transform.rotation);
            Reconciliate(reconData, true);
        }
    }

    // used for rigidboddies and physic related stuff.
    private void TimeManager_OnPostTick()
    {
        
    }

    private void GetInput(out MoveData moveData)
    {
        moveData = default;

        var movementInput = _playerControls.DefaultMap.Move.ReadValue<Vector2>();

        float horizontal = movementInput.x;
        float vertical = movementInput.y;

        if (horizontal == 0f && vertical == 0f) return;
        moveData = new MoveData(horizontal, vertical);
    }

    [Replicate]
    private void Move(MoveData moveData, bool asServer, bool replaying = false)
    {
        Vector3 move = new Vector3(moveData.Horizontal, 0f, moveData.Vertical);
        _characterController.Move(move * MovementSpeed * (float)base.TimeManager.TickDelta);
    }

    [Reconcile]
    private void Reconciliate(ReconcileData recData, bool asServer)
    {
        transform.position = recData.Position;
        transform.rotation = recData.Rotation;
    }

}
