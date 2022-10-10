using UnityEngine;
using UnityEngine.UI;

public class CooldownData
{
    public CooldownData(IHasCooldown cooldown)
    {
        Id = cooldown.Id ;
        RemainingTime = cooldown.CooldownDuration ;
        Cooldown = cooldown.CooldownDuration ;
        Image = cooldown.Image ;
    }
    

    public string Id { get; }
    public float RemainingTime { get; set;}
    public float Cooldown { get; set;}
    public Image Image { get; set;}


    public bool DecrementalCooldown(float deltaTime)
    {
        RemainingTime = Mathf.Max(RemainingTime - deltaTime, 0.0f) ;

        Image.fillAmount =  RemainingTime / Cooldown ;
        
        return RemainingTime == 0.0f ;
    }

}
