using System.Threading.Tasks;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using Udar.SceneField;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance { get; private set; }

    [SerializeField] private GameObject _loadingCanvas;
    [SerializeField] private Image _progressBar;
    private float _target;
    private bool _isLoading;

    [Header("Landing Page")]
    [SerializeField] private SceneField _landingPage;
    [Header("Stage 01 Level 01")]
    [SerializeField] private SceneField _stage01Level01;

    [Header("Empty Scene")]
    [SerializeField] private SceneField _emptyScene;

    private void Awake()
    {
        // ! Destroy Latest
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
            
        else
            Instance = this;

        DontDestroyOnLoad(this.gameObject);
        
        _isLoading = false;
        _loadingCanvas.SetActive(_isLoading);

    }

    public async void LoadScene(SceneField sceneToLoad)
    {
        _isLoading = true;
        _target = 0.0f;
        _progressBar.fillAmount = 0.0f;

        var scene = SceneManager.LoadSceneAsync(sceneToLoad.SceneName);
        scene.allowSceneActivation = false ;

        _loadingCanvas.SetActive(_isLoading);

        do
        {
            await Task.Delay(100);
            _target = scene.progress / 0.9f;
        } while (_target < 1.0f);


        await Task.Delay(500);
        scene.allowSceneActivation = true;
        await Task.Delay(1);
        _isLoading = false;
        _loadingCanvas.SetActive(_isLoading);

    }

    private void Update()
    {
        if (_isLoading == false)
            return;
        
        _progressBar.fillAmount = Mathf.MoveTowards(
                                                    _progressBar.fillAmount, 
                                                    _target, 
                                                    3f * Time.deltaTime
                                                    );
    }


}
