using UnityEngine;

public interface IHasCooldown
{
    string Id { get; }
    
    float CooldownDuration {get;}
}
