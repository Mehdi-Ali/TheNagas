using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  GameAssets : MonoBehaviour
{
    //the game object holding this script must be in the resources folder
    private static GameAssets _i ;

    public static GameAssets i 
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i ;
        }
    }


    public Transform DamagePopUp ;
}
