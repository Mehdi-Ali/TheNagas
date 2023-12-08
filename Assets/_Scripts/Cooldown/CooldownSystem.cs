using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

public class CooldownSystem : NetworkBehaviour
{
    private readonly List<CooldownData> cooldowns = new();

    #if !UNITY_SERVER
    public Dictionary<string, Image> ImageDictionary =  new();
    #endif

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        SubscribeToTimeManager(true);
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        SubscribeToTimeManager(false);
        if (IsOffline)
            ImageDictionary.Clear();
    }

    private void TimeManager_OnTick()
    {
        ProcessCooldowns();
    }
    private void ProcessCooldowns()
    {
        float tickDelta = (float)TimeManager.TickDelta ;

        for (int i = cooldowns.Count - 1; i >= 0 ; i--)
        {
            var remainingTime = cooldowns[i].DecrementalCooldown(tickDelta);

            if (IsOwner)
            {
                if(ImageDictionary[cooldowns[i].Id])
                    ImageDictionary[cooldowns[i].Id].fillAmount = remainingTime / cooldowns[i].Cooldown;
            }

            if (remainingTime == 0.0f )
                cooldowns.RemoveAt(i) ;
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
            if (cd.Id == id) return cd.RemainingTime ;
        }

        return 0.0f;
    }

    private void SubscribeToTimeManager(bool subscribe)
    {
        if (subscribe)
            base.TimeManager.OnTick += TimeManager_OnTick;

        else
            base.TimeManager.OnTick -= TimeManager_OnTick;
    }

    // public void RestartCooldown(IHasCooldown cd)
    // {
    //     foreach (CooldownData cooldown in cooldowns)
    //     {
    //         if (cooldown.Id == cd.Id)
    //         {
    //             cooldown.RemainingTime = cd.CooldownDuration;
    //             return;
    //         }
    //     }

    //     PutOnCooldown(cd);
    // }

}
