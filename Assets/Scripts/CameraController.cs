using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner) return;

        gameObject.SetActive(true);
    }
}
