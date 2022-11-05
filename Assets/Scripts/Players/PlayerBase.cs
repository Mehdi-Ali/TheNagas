using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerBase : NetworkBehaviour // make a damageable Enemy and player 
{
    private PlayerStateManger _player ;
    private HealthBar _healthBar ;

    [SyncVar] private float _health ;

    [SyncVar] private float _maxHealth ;
    
    
    //Events
    public event Action OnDie ;
    public event Action OnDamage ;
    public event Action OnHeal ;


    public virtual void Awake()
    {
        _player = GetComponent<PlayerStateManger>();
        _healthBar = GetComponentInChildren<HealthBar>();

    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _maxHealth = _player.Statics.MaxHealth ;
        _health = _maxHealth;

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
        _healthBar.SetHealth(health);
        DamagePopUp.Create(transform.position, damage, Color.red);
    }

    [Server]
    public virtual void Die()
    {
        OnDie?.Invoke();
        _player.SwitchState(_player.DeadState);

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