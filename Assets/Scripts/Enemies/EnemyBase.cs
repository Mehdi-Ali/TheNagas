using UnityEngine;

public class EnemyBase : Damageable
{
    // [SerializeField] bool _takeDamage; // dell ; just a test
    [SerializeField] float _maxHealth = 100.0f;

    public override float MaxHealth { get => _maxHealth; }
    public override float Health { get; set; }


    public override void Awake()
    {
        base.Awake();

        // Get Stat Class
        // _maxHealth = stats.MaxHealth ;
    }

    private void Update()
    {
        // // dell ; just a test
        // if (_takeDamage) TakeDamage(1.0f);
        // Debug.Log(Health);
    }
}
