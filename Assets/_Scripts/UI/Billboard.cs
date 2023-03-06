using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Managing.Scened;
using System;

public class Billboard : MonoBehaviour
{
    private Transform _cameraTransform ;
    private bool isInGameplay;


    void LateUpdate()
    {
        if (_cameraTransform == null)
        {
            var mainCamera = Camera.main;

            if (mainCamera != null)
                _cameraTransform = mainCamera.transform;
        }

        if (_cameraTransform == null)
            return;

        transform.LookAt(transform.position + _cameraTransform.forward);
    }

    // void Awake()
    // {
    //     InstanceFinder.SceneManager.OnLoadEnd += SceneManagerOnLoadEnd;
    //     InstanceFinder.SceneManager.OnLoadStart += SceneManagerOnLoadStart;

    // }

    // private void SceneManagerOnLoadStart(SceneLoadStartEventArgs obj)
    // {
    //     isInGameplay = false;
    // }

    // private void SceneManagerOnLoadEnd(SceneLoadEndEventArgs obj)
    // {
    //     isInGameplay = true;
    // }

    // void OnDestroy()
    // {
    //     InstanceFinder.SceneManager.OnLoadEnd -= SceneManagerOnLoadEnd;
    //     InstanceFinder.SceneManager.OnLoadStart -= SceneManagerOnLoadStart;


    // }
}
