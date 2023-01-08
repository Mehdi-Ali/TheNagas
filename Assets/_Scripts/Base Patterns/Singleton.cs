using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    private enum PersistanceType
    {
        DestroyNewest,
        DestroyOldest,
        AllowMultiple
    };

    [SerializeField] private PersistanceType _persistance ;

    private void Awake()
    {
        HandlePersistance(_persistance);
    }

    private void HandlePersistance(PersistanceType persistance)
    {
        switch (persistance)
        {
            case PersistanceType.DestroyNewest:
                DestroyNewest();
                break;

            case PersistanceType.DestroyOldest:
                DestroyOldest();
                break;

            case PersistanceType.AllowMultiple:
                break;

            default:
                DestroyNewest();
                break;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void DestroyNewest()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);

        else
            Instance = this as T;
    }

    private void DestroyOldest()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance.gameObject);

        Instance = this as T;
    }

}
