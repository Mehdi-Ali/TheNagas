using FishNet.Object;
using UnityEngine;

public class Player : NetworkBehaviour
{
    
    public static Player Instance {get; private set;}

    // try remove the field: should be okay.
    [field: SerializeField]
    public int Score
    {
        get;

        [ServerRpc]
        private set;
    }

    [SerializeField] bool KEEPCHANGINGSCORE;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
            return;

        Instance = this;

        ViewsManager.Instance.Initialize();
    }

    void Update()
    {
        if (!IsOwner)
            return;
        
        if (KEEPCHANGINGSCORE)
            Score = Random.Range(0, 1024);
    }
}
