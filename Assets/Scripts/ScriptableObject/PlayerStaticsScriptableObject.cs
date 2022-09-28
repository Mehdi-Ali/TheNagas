using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatics", menuName = "ScriptableObject/Player/PlayerStatics")]
public class PlayerStaticsScriptableObject : ScriptableObject
{
    public float MaxHealth ;
    public float AutoAimRange ;
}
