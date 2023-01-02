using System.Collections;
using System.Collections.Generic;
using Udar.SceneField;
using UnityEngine;

public class ChangeSceneButton : MonoBehaviour
{
    [Header("Scene To Load")]
    [SerializeField] private SceneField _sceneToLoad;

    public void ChangeScene()
    {
        MySceneManager.Instance.LoadScene(_sceneToLoad);
    }
}
