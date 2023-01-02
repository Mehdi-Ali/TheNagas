using UnityEngine;

public class SingletonSpriteManager : MonoBehaviour
{

    public static SingletonSpriteManager Instance { get; private set; }
    private void Awake() 
    {        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public GameObject KnightUltCracks = null;
    // TODO make a system that use the sprites and if the sprites are already in use, it instantiate an other one (simple pooling)

}
