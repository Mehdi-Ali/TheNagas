using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerBase : NetworkBehaviour // make a damageable Enemy and player 
{
    private PlayerStateManger _player ;
    private HealthBar _healthBar ;

    // instead we can put this script on an empty gameObject and put with it all the script that 
    // works only when IsAlive == true and disable it when dead. 
    [SyncVar] public bool IsAlive; 
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
        _healthBar.SetHealth(health);
        DamagePopUp.Create(transform.position, damage, Color.red);
    }

    [Server]
    public virtual void Die()
    {
        OnDie?.Invoke();
        IsAlive = false ;
        RpcOnDie();
    }

    [ObserversRpc(RunLocally = true)]
    private void RpcOnDie()
    {
        _player.SwitchState(_player.DeadState);
        
        if (!IsClient) return;
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