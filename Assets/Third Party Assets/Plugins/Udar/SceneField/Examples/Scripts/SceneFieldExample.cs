using UnityEngine;

namespace Udar.SceneField.Example
{
    public class SceneFieldExample : MonoBehaviour
    {
        [SerializeField] private SceneField _sceneField;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                Debug.Log("The scene name is: " + _sceneField.SceneName);
                Debug.Log("The scene index is: " + _sceneField.BuildIndex);
                Debug.Log("The scene path is: " + _sceneField.ScenePath);
            }
        }

        public void PrintSceneField_Button(SceneFieldRef sceneFieldRef)
        {
            Debug.Log("-------------------------------------------");
            Debug.Log("Scene Field Name= " + sceneFieldRef.SceneField.SceneName);
        }
    }
}

