using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM : MonoBehaviour
{

    enum State
    {
        STATE_Idle,
        STATE_Runing,
        STATE_AutoAttack,
        STATE_FirstAbility,
        STATE_SecondAbility,
        STATE_ThirdAbility,
        STATE_Ultimate,
        STATE_Death
    };

}
