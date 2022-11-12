using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicConfig : MonoBehaviour
{
    [SerializeField] private int FPS ;
    [SerializeField] private int FPvSync ;

    void start()
    {
        QualitySettings.vSyncCount = FPvSync ;
        Application.targetFrameRate = FPS ;
    }

}
