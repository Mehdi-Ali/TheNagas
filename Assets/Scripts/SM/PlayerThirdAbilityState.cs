using UnityEngine;

public class PlayerThirdAbilityState : PlayerBaseState
{
    //Ability Name
    public string AbilityName = "Dash" ;

    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] float _dashSpeed = 5.0f;

    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _thirdAbility;

    private void Awake()
    {
        //Caching The Player State Manger
        player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _thirdAbility = Animator.StringToHash("ThirdAbility");
    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        Invoke(nameof(AttackComplete), 1f);
        player.NetworkAnimator.SetTrigger(_thirdAbility);
       // transform.Translate(Vector3.forward * _dashSpeed * Time.deltaTime );
        player.ReadyToSwitchState = false;
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
        player.ReadyToSwitchState = true;
        player.IsCastingAnAbility = false;
        player.SwitchState(player.IdleState);
    }
}
