using UnityEngine;

public sealed class ViewsManager : MonoBehaviour
{
    public static ViewsManager Instance { get; private set; }

    [SerializeField] private bool _autoInitialize;
    [SerializeField] private View _defaultView;

    [SerializeField] private View[] _views;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);

        else
            Instance = this;
    }

    private void Start()
    {
        if (_autoInitialize)
            Initialize();
    }

    public void Initialize()
    {
        _views = Resources.FindObjectsOfTypeAll<View>();

        foreach (var view in _views)
        {
            view.Initialize();
            view.Hide();
        }

        _defaultView?.Show();
    }

    // add an argument that enables to just lower the opacity of the active view
    // instead of hiding it completely.
    public void Show<TView>(object args = null) where TView : View
    {
        foreach (var view in _views)
        {
            if (view is TView)
                view.Show(args); 

            else 
                view.Hide();
        }
    }


}
