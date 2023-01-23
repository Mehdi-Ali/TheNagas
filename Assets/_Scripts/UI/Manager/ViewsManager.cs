using System;
using FishNet;
using UnityEngine;

public sealed class ViewsManager : MonoBehaviour
{
    public static ViewsManager Instance { get; private set; }

    [SerializeField] private bool _autoInitialize;
    [SerializeField] private View _defaultView;
    public View CurrentView;

    [SerializeField] private View[] _views;

    private void Awake()
    {
        // ! Destroy Oldest
        if (Instance != null && Instance != this)
            Destroy(Instance.gameObject);

        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (_autoInitialize)
            Initialize();
    }

    public void Initialize()
    {
        _views = FindObjectsOfType<View>(true);

        foreach (var view in _views)
        {
            view.Initialize();
            view.Hide();
        }

        CurrentView = _defaultView;
        _defaultView?.Show();
    }

    // Should make a method in the view manager that goes back to the previous view

    public void Show<TView>(object args = null) where TView : View
    {
        foreach (var view in _views)
        {
            if (view is TView)
            {
                CurrentView = view ;
                view.Show(args); 
            }

            else 
                view.Hide();
        }

        // this is usefully if we want to leave the latest view in the background
        // the newest view to show should have a black background with 0.5 alpha.
        if (args is bool leaveBackGround && leaveBackGround)
        {
            foreach (var view in _views)
            {
                if (view is TView)
                    view.Show(args);
            }
        }
    }

    public void UpdateCurrentView()
    {
        if (!InstanceFinder.IsOffline)
            CurrentView?.UpdateView();
    }
}
