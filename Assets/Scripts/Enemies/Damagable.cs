using System;
using UnityEngine;

public abstract class Damagable : MonoBehaviour
{
    
    abstract public float Maxhealth { get; }
    abstract public float Health { get ; set ;}

    public event Action OnDied ;

    private void Awake() // make virttual 
    {
        Health = Maxhealth;
    }

    public void TakeDamage(float damage)
    {
        Health = Mathf.Max(0.0f, Health - damage);

        if (Health == 0) Die();
    }

    public virtual void Die()
    {
        OnDied?.Invoke();
        Debug.Log("Dead");
    }

    public virtual void GetHeal(float heal)
    {
        Health = Mathf.Max(Maxhealth, Health + heal);

        if (Health == Maxhealth) Getsheild() ;
    }

    public virtual void Getsheild()
    {

    }

}
