using UnityEngine;

public class PlayerSecondAbilityState : PlayerBaseState
{
    //Name of The Abbility
    public string AbbilityName = "Slash";

    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] private float _animationSpeed = 2.0f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _secondAbilityHash;
    int _secondAbility_Multiplier ;

    private void Awake()
    {       
         //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _secondAbilityHash = Animator.StringToHash("SecondAbility");
        _secondAbility_Multiplier = Animator.StringToHash("SecondAbility_Multiplier");
       
       _player.Animator.SetFloat("SecondAbility_Multiplier", _animationSpeed);
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        Invoke(nameof(AttackComplete), _player.AnimationsLength.SecondAbilityDuration / _animationSpeed);
        
        // Set player rotation to the hitbox rotation.
        // and rest the hit box rotation so that it seems that it didn't rotate.
        
        _player.Animator.CrossFade(_secondAbilityHash, 0.1f);
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        //enable SwitchState


        // should go to the methode that deals dmg
    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false ;
        _player.SwitchState(_player.IdleState);
    }

}
