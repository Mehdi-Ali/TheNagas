using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicConfig : MonoBehaviour
{
    [SerializeField] private int FPS ;

    void start()
    {
        QualitySettings.vSyncCount = 0 ;
        Application.targetFrameRate = FPS ;
    }

}
