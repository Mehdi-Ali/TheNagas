using System.Collections.Generic;
using UnityEngine;

public class PlayerUltimateState : PlayerBaseState, IHasCooldown
{
    //Name of The Abbility
    public string AbilityName = "Dank";

    //Game Designe Vars, Mak a stat Script maybe
    [SerializeField] private float _animationSpeed = 1.5f;
    [SerializeField] public float Range = 5.0f;
    [SerializeField] float _cooldown = 5.0f;
    [SerializeField] float _damage = 99.0f;


    //Variables...
    bool _grounded ;
    float _tLerp ;
    Vector3 _end;
    Vector3 _start;


    //Cashing the Player State Manager : Should do to all state scripts 
    PlayerStateManger _player;

    //Variables to store omptimized Setter / getter parameter IDs
    int _ultimateHash;
    int _ultimateMultiplierHash;

    // cooldown things
    public string Id => AbilityName;
    public float CooldownDuration => _cooldown;

    //Storing Variables
    private HashSet<EnemyBase> _targets = new HashSet<EnemyBase>();

    private void Awake()
    {
        //Caching The Player State Manger
        _player = GetComponent<PlayerStateManger>();

        //caching Hashes
        _ultimateHash = Animator.StringToHash("Ultimate");
        _ultimateMultiplierHash = Animator.StringToHash("Ultimate_Multiplier");

    }

    public override void EnterState()
    {
        if (!base.IsOwner) return;
        //check cooldown
        _player.CooldownSystem.PutOnCooldown(this);

        _grounded = false;
        _tLerp = 0.0f ;
        _start = transform.position ;
        _end = _player.HitBoxes.transform.position ;


        Invoke( nameof(AttackComplete),
                (_player.AnimationsLength.UltimateDuration / _animationSpeed ));
        
        _player.Animator.SetFloat(_ultimateMultiplierHash, _animationSpeed);
        _player.Animator.CrossFade(_ultimateHash, 0.1f);

        //activating colider
        _player.ReadyToSwitchState = false;
        _player.IsCastingAnAbility = true;
    }

    public override void UpdateState()
    {
  
        
       
        if (_grounded) return;
        _tLerp += Time.deltaTime * _animationSpeed / ( _player.AnimationsLength.UltimateDuration - ((41f - 28f) / 30f ));
        transform.position = Vector3.Lerp( _start, _end, _tLerp );

    }

    public override void ExitState()
    {

    }

    void AttackComplete()
    {
        _player.ReadyToSwitchState = true;
        _player.IsCastingAnAbility = false;
        _player.SwitchState(_player.IdleState);
    }

    void UltimateEvent()
    {
        foreach(EnemyBase enemy in _targets)
        {
            enemy.TakeDamage(_damage);
        }

        _player.ActiveAttackCollider.gameObject.SetActive(false) ;
        _targets.Clear() ;
    }

        private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyBase>(out EnemyBase damageableEnemy))
            {
                _targets.Add(damageableEnemy);
            }
    }


    void StopUltimateTransformEvent()
    {
        _grounded = true ;
    }
}
