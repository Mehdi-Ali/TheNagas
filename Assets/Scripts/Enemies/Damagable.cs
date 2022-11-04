using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Damageable : NetworkBehaviour
{
    //Variables to cache Instances
    private EnemyStateManger _enemy ;
    private HealthBar _healthBar ;

    // synched vars
    [SyncVar] private float _health ;
    private float _shield ;


    // utilities
    private float _maxHealth ;
    
    
    //Events
    public event Action OnDied ;
    public event Action OnDamaged ; // dele



    public virtual void Awake()
    {
        _enemy = GetComponent<EnemyStateManger>();
        _healthBar = GetComponentInChildren<HealthBar>();

        _maxHealth = _enemy.Statics.MaxHealth ;

        _health = _maxHealth;
        _healthBar.SetMaxHealth(_maxHealth) ;

    }

    

    [Server]
    public void TakeDamage(float damage)
    {
        _health = Mathf.Max(0.0f, _health - damage);

        if (_health == 0) Die();

        _healthBar.SetHealth(_health);
        
        DamagePopUp.Create(this.transform.position , damage, Color.yellow);

    }

    [Server]
    public virtual void Die()
    {
        OnDied?.Invoke();
        _enemy.SwitchState(_enemy.DeadState);
        
        _healthBar.gameObject.SetActive(false);

    }


    [Server]
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

    [Server]
    public virtual void GetShield(float shield)
    {

        // TODO implement later: make something like a real
        // TODO health which is eq to = health + shield
    }

}
