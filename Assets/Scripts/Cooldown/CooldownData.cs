using UnityEngine;

public class CooldownData 
{
    public CooldownData(IHasCooldown cooldown)
    {
        Id = cooldown.Id ;
        RemainingTime = cooldown.CooldownDuration ;
        Cooldown = cooldown.CooldownDuration ;
    }
    

    public string Id { get; }
    public float RemainingTime { get; set;}
    public float Cooldown { get; set;}


    public float DecrementalCooldown(float deltaTime)
    {
        RemainingTime = Mathf.Max(RemainingTime - deltaTime, 0.0f) ;
        
        return RemainingTime;
    }

}
