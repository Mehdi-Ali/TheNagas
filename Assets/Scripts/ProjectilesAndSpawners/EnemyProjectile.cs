using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    //SetUp
    private Action<EnemyProjectile> _collidedAction ;

    //Proporties 
    public float Damage ;
    public float Speed ;
    public float Range ;

    // Utilities
    private float _timeOut ;


    public void Init(Action<EnemyProjectile> CollidedAction)
    {
        _collidedAction = CollidedAction ;
        _timeOut = Range / Speed ;
        StartCoroutine(Destroy());

    }


    void Update()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider collider)
    {

        if (collider.TryGetComponent<PlayerBase>(out PlayerBase playerBase))
        {
            playerBase.TakeDamage(Damage) ;
            _collidedAction(this);      
        }

        // it does nothing when it collides with an enemy and is destroyed when it touches anything else 
        // else if (collider.TryGetComponent<EnemyBase>(out EnemyBase enemyBase)) 
        // {
        //     return;
        // }

        // else // if it's not static 
        // {
        //     _collidedAction(this);
        // }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(_timeOut);
        _collidedAction(this);
    }

}
