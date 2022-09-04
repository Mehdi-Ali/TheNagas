using System;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    //Abstract Parameters
    abstract public float MaxHealth { get; }
    abstract public float Health { get ; set ;}
    public Animator Animator { get; set; }

    //Variables to store optimized Setter / getter parameter IDs
    int _DeadHash ;

    // del
    private float _shield ;
    
    
    //Events
    public event Action OnDied ;
    public event Action OnDamaged ; // dele



    public virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        _DeadHash = Animator.StringToHash("Dead");

        Health = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        Health = Mathf.Max(0.0f, Health - damage);

        if (Health == 0) Die();
        
        //delete
        Debug.Log(Health);
     }

    public virtual void Die()
    {
        OnDied?.Invoke();

        Animator.CrossFade(_DeadHash, 0.1f);
        Debug.Log("Dead");
    }

    public virtual void GetHeal(float heal)
    {
        if ( Health + heal >= MaxHealth) 
        {
            _shield = MaxHealth - Health + heal ;
            Health = MaxHealth ;
            GetShield(_shield) ;
        }

        else
        {
            Health = Health + heal ;
        }
    }

    public virtual void GetShield(float shield)
    {

        // TODO implement later: make something like a real
        // TODO health which is eq to = health + shield
    }

}
