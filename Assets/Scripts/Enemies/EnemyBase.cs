using UnityEngine;

public class EnemyBase : Damageable
{
    //
    private EnemyStatics _enemyStatics ;
    private HealthBar _healthBar ; //TODO  to State Machine
    
    public override float MaxHealth { get; set; }
    public override float Health { get; set; }


    public override void Awake()
    {
        base.Awake();

        _enemyStatics = GetComponent<EnemyStatics>() ;
        _healthBar = FindObjectOfType<HealthBar>(); //TODO fix and put in state Machine


        MaxHealth = _enemyStatics.MaxHealth ;
        _healthBar.SetMaxHealth(MaxHealth) ;
    }

    private void Update()
    {

    }
}
