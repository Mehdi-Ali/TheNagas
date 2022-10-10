using UnityEngine;
using UnityEngine.UI;

public interface IHasCooldown
{
    string Id { get; }
    
    float CooldownDuration {get;}

    public Image Image { get; }
}
