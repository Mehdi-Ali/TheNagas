using System;
using UnityEngine;

public class Damageable : MonoBehaviour // make a damageable Enemy and player 
{
    //Variables to cache Instances
    private EnemyStatics _enemyStatics ;
    public Animator Animator;
    private HealthBar _healthBar ;



    //Variables to store optimized Setter / getter parameter IDs
    int _DeadHash ;

    // utilities
    private float _maxHealth ;
    private float _health ;
    private float _shield ;
    
    
    //Events
    public event Action OnDied ;
    public event Action OnDamaged ; // dele



    public virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        _DeadHash = Animator.StringToHash("Dead");
        _enemyStatics = GetComponent<EnemyStatics>() ;

        _healthBar = GetComponentInChildren<HealthBar>();

        _maxHealth = _enemyStatics.MaxHealth ;

        _health = _maxHealth;
        _healthBar.SetMaxHealth(_maxHealth) ;

        


    }

    public void TakeDamage(float damage)
    {
        _health = Mathf.Max(0.0f, _health - damage);

        if (_health == 0) Die();

        _healthBar.SetHealth(_health);
        //popup dmg number
        
        //delete
        Debug.Log(_health);
     }

    public virtual void Die()
    {
        OnDied?.Invoke();
        GetComponent<CapsuleCollider>().enabled = false ;
        Animator.CrossFade(_DeadHash, 0.1f);
        
        Debug.Log("Dead");
        _healthBar.gameObject.SetActive(false) ;

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
