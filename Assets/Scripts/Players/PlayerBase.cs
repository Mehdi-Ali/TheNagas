using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerBase : NetworkBehaviour // make a damageable Enemy and player 
{
    //Variables to cache Instances
    private PlayerStateManger _player ;
    private Animator _animator;
    private HealthBar _healthBar ;

    // synched vars
    [SyncVar] private float _health ;
    private float _shield ;


    //Variables to store optimized Setter / getter parameter IDs
    int _DeadHash ;

    // utilities
    private float _maxHealth ;
    private Vector3 _position;
    
    
    //Events
    public event Action OnDied ;
    public event Action OnDamaged ; // dele



    public virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _DeadHash = Animator.StringToHash("Dead");
        _player = GetComponent<PlayerStateManger>();

        _healthBar = GetComponentInChildren<HealthBar>();
        _position = this.transform.position;

        _maxHealth = _player.Statics.MaxHealth ;

        _health = _maxHealth;
        _healthBar.SetMaxHealth(_maxHealth) ;

    }

    public void TakeDamage(float damage)
    {
        _health = Mathf.Max(0.0f, _health - damage);
        _healthBar.SetHealth(_health);
        DamagePopUp.Create(_position , damage, Color.red);   

        if (_health == 0) Die();
     }

    public virtual void Die()
    {
        OnDied?.Invoke();
        _player.SwitchState(_player.DeadState);
        
        _healthBar.gameObject.SetActive(false);
    }

    public virtual void GetHeal(float heal)
    {
        if ( _health + heal >= _maxHealth) 
        {
            _shield = _maxHealth - _health + heal ;
            _health = _maxHealth ;
            GetShield(_shield) ;
        }

        else
        {
            _health = _health + heal ;
        }
    }

    public virtual void GetShield(float shield)
    {

        // TODO implement later: make something like a real
        // TODO health which is eq to = health + shield
    }

}