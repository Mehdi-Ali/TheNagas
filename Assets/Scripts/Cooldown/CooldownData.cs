using UnityEngine;

public class CooldownData
{
    public CooldownData(IHasCooldown cooldown)
    {
        Id = cooldown.Id ;
        ReminingTime = cooldown.CooldownDuration ;
    }
    

    public string Id { get; }
    public float ReminingTime { get; set;}


    public bool DecrementalCooldown(float deltaTime)
    {
        ReminingTime = Mathf.Max(ReminingTime - deltaTime, 0.0f) ;
        
        return ReminingTime == 0.0f ;
    }

}
