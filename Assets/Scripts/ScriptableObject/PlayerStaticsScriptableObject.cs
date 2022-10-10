using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatics", menuName = "ScriptableObject/Player/PlayerStatics")]
public class PlayerStaticsScriptableObject : ScriptableObject
{
    [Header("General")]

    public float MaxHealth ;
    public float MovementSpeed;
    public float RotationSpeed;

    [Space(10)]

    [Header("Auto Attack")]

    public string AutoAttackAbilityName;
    public float AutoAttackAnimationSpeed;
    public float AutoAttackDashingMovementSpeed;
    public float AutoAttackDashingTime;
    public float AutoAttackStopDistance ;
    public float AutoAttackRotationSpeed;
    public float AutoAttackDamage1;
    public float AutoAttackDamage2;
    public float AutoAttackDamage3;
    public float AutoAttackRange;

    [Space(10)]

    [Header("First Ability")]

    public string FirstAbilityAbilityName  ;
    public float FirstAbilityAnimationSpeed  ;
    public float FirstAbilityCooldown ;
    public float FirstAbilityDamage;
    public float FirstAbilityRange;
    public float FirstAbilityMovementSpeed;
    public int FirstAbilityTicks ;

    [Space(10)]

    [Header("Second Ability")]

    public string SecondAbilityAbilityName  ;
    public float SecondAbilityAnimationSpeed  ;
    public float SecondAbilityCooldown ;
    public float SecondAbilityDamage;
    public float SecondAbilityRange;

    [Space(10)]

    [Header("Third Ability")]

    public string ThirdAbilityAbilityName  ;
    public float ThirdAbilityAnimationSpeed  ;
    public float ThirdAbilityCooldown ;
    public float ThirdAbilityDamage;
    public float ThirdAbilityRange;

    [Space(10)]

    [Header("Ultimate Ability")]

    public string UltimateAbilityAbilityName  ;
    public float UltimateAbilityAnimationSpeed  ;
    public float UltimateAbilityCooldown ;
    public float UltimateAbilityDamage;
    public float UltimateAbilityRange;


}
