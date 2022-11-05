using System;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class EnemyBase : NetworkBehaviour
{
    private EnemyStateManger _enemy ;
    private HealthBar _healthBar ;

    [SyncVar] public bool IsAlive;
    [SyncVar] private float _health ;

    [SyncVar] private float _maxHealth ;
    
    // TODO check FishNet Broadcast may be they are the Networking standard for events
    public event Action OnDie ;
    public event Action OnDamage ;
    public event Action OnHeal ;


    public virtual void Awake()
    {
        _enemy = GetComponent<EnemyStateManger>();
        _healthBar = GetComponentInChildren<HealthBar>();

    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        _maxHealth = _enemy.Statics.MaxHealth ;
        _health = _maxHealth;
        IsAlive = true ;

        _healthBar.SetMaxHealth(_maxHealth) ;
    }

    [Server]
    public void TakeDamage(float damage)
    {
        _health = Mathf.Max(0.0f, _health - damage);

        OnDamage?.Invoke();

        RpcOnDamage(_health, damage);

        if (_health == 0) Die();

    }

    [ObserversRpc]
    private void RpcOnDamage(float health, float damage)
    {
        // TODO to move to a different script more oriented to sound, vfx, display things...

        _healthBar.SetHealth(health);

        DamagePopUp.Create(transform.position, damage, Color.yellow);
    }


    [Server]
    public virtual void Die()
    {
        OnDie?.Invoke();
        IsAlive = false ;
        _enemy.SwitchState(_enemy.DeadState);

        RpcOnDie();
    }

    [ObserversRpc]
    private void RpcOnDie()
    {
        _healthBar.gameObject.SetActive(false);
    }

    [Server]
    public virtual void GetHeal(float heal)
    {
        _health = Mathf.Max(_health + heal, _maxHealth);

        RpcOnHeal(heal);

    }

    [ObserversRpc]
    private void RpcOnHeal(float heal)
    {
        DamagePopUp.Create(transform.position, heal, Color.green);
    }
}
