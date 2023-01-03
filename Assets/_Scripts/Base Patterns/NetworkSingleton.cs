
using FishNet.Object;
using UnityEngine;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T Instance { get; private set; }
    private NetworkObject _networkObject;

    private enum PersistanceType
    {
        DestroyNewest,
        DestroyOldest,
        AllowMultiple
    };

    [SerializeField] private PersistanceType _persistance;

    private void Awake()
    {
        HandlePersistance(_persistance);
        _networkObject = GetComponent<NetworkObject>();
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