using UnityEngine;

public class EnemyBase : Damagable
{
    [SerializeField] bool _takeDamage ;
    [SerializeField] float _maxHealth = 100.0f ;

    public override float Maxhealth { get => _maxHealth; }
    public override float Health { get ; set ; }


    private void Update()
    {
        if(_takeDamage) TakeDamage(1.0f);
        Debug.Log(Health);
    }
}
