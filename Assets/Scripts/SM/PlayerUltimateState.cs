using UnityEngine;

public class PlayerUltimateState : PlayerBaseState
{
    //Name of The Abbility
    public string AbbilityName = "Dank";

    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] private float _animationSpeed = 1.0f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _ultimateHash;
    int _ultimateMultiplierHash;

    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _ultimateHash = Animator.StringToHash("Ultimate");
        _ultimateMultiplierHash = Animator.StringToHash("Ultimate_Multiplier");

        _player.Animator.SetFloat(_ultimateMultiplierHash, _animationSpeed);

    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        Invoke(nameof(AttackComplete),_player.AnimationsLength.UltimateDuration / _animationSpeed);
        _player.Animator.CrossFade(_ultimateHash, 0.1f);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        //enable SwitchState
    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
    }

    void UltimateEvent()
    {
        Debug.Log("Biger Pop");
        //Activat Collider + deal damage 
    }
}
