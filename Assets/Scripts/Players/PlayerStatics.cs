using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatics : MonoBehaviour
{
    public float MaxHealth = 100.0f;
    public float AutoAimRange = 6.0f;


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, AutoAimRange);
    }
}
