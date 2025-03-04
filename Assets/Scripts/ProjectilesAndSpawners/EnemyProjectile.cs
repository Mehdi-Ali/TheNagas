using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EnemyProjectile : NetworkBehaviour 
{

    public float Damage ;
    public float Speed ;
    public float Range ;
    private float _timeOut = 1f ;
    private Vector3 _uniVect = new(1f, 1f, 1f);


    public override void OnStartServer()
    {
        base.OnStartServer();
        SubscribeToTimeManager(true);
    }

    public IEnumerator OnSpawned()
    {
        transform.localScale = _uniVect;
        _timeOut = Range / Speed ;
        yield return new WaitForSeconds(_timeOut);
        Despawn(DespawnType.Pool);
    }

    private void TimeManager_OnTick()
    {
        transform.Translate(Vector3.forward * Speed * (float)TimeManager.TickDelta);
        //transform.localScale = Vector3.Lerp(_uniVect, Vector3.zero, _timeOut);
    }

    [Server]
    private void OnTriggerEnter(Collider collider)
    {

        if (collider.TryGetComponent<PlayerBase>(out PlayerBase playerBase))
        {
            if (!playerBase.IsAlive) return;
            playerBase.TakeDamage(Damage) ;
            Despawn(DespawnType.Pool);
        }

        // TODO Make the projectile Despawn if it collides with a static object.
        // else if (collider.gameObject.isStatic)
        // {
        //     Despawn(DespawnType.Pool);
        // }

    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        SubscribeToTimeManager(false);
        
    }
    private void SubscribeToTimeManager(bool subscribe)
    {
        if (base.TimeManager == null) return;

        if (subscribe)
            base.TimeManager.OnTick += TimeManager_OnTick;

        else
            base.TimeManager.OnTick -= TimeManager_OnTick;
    }

}
