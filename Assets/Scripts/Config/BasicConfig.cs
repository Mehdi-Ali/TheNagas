using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicConfig : MonoBehaviour
{
    [SerializeField] private int FPS ;

    void Awake()
    {
        Application.targetFrameRate = FPS ;
    }

}
