using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownSystem : MonoBehaviour
{
    private readonly List<CooldownData> cooldowns = new List<CooldownData>();

    private void Update() 
    {
        ProcessCooldowns();    
    }

    private void ProcessCooldowns()
    {
        float deltaTime = Time.deltaTime ;
        
        for (int i = cooldowns.Count - 1; i >= 0 ; i--)
        {
            if (cooldowns[i].DecrementalCooldown(deltaTime) ) cooldowns.RemoveAt(i) ;
        }

    }

    public void PutOnCooldown(IHasCooldown cd)
    {
        cooldowns.Add(new CooldownData(cd)) ;
    }

    public bool IsOnCooldown(string id)
    {
        foreach (CooldownData cd in cooldowns)
        {
            if (cd.Id == id ) return true ;
        }

        return false;
    }

    public float GetRemainingDuration(string id)
    {
        foreach (CooldownData cd in cooldowns)
        {
            if (cd.Id == id) return cd.ReminingTime ;
        }

        return 0.0f;
    }


}
