using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatics", menuName = "ScriptableObject/Player/PlayerStatics")]
public class PlayerStaticsScriptableObject : ScriptableObject
{
    public float MaxHealth ;


    //FirstAbilityState
    public float FirstAbilityStateRange;

    //SecondAbilityState
    public float SecondAbilityStateRange;

    //ThirdAbilityState
    public float ThirdAbilityStateRange;

    //UltimateAbilityState
    public float UltimateAbilityStateRange;


}
