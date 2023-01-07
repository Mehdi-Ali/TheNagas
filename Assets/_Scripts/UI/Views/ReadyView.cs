using FishNet;
using UnityEngine;
using UnityEngine.UI;

public class ReadyView : View
{
    [SerializeField] private Button _readyButton;

    public override void Initialize()
    {
        _readyButton.onClick.AddListener(() =>
        {
            ViewsManager.Instance.Show<GamePlayView>();
            // TODO :
            // Invoke the start event to spawn the Player's Character and start the gamePlay
            // make a singleton that handles all events ?
            // or better yet use the Scrutable object Event Handler pattern (Jason)

            // ! check this 
            Player.LocalPlayer.ServerSpawnCharacter();
            Debug.Log("Ready view call");

        });

        base.Initialize();
    }
}
