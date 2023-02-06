using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;

public class Portal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerBase>(out PlayerBase _))
        {
            if (!InstanceFinder.IsServer)
                return;
            
            Debug.Log("Portal activated");
            StageManager.Instance.ServerNextLevel();
        }
        
    }
}
