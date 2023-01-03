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
            // Invoke the start event to spawn the player Knight and start the gamePlay
            // make a singleton that handles all events ?
            // or better yet use the Scrutable object Event Handler pattern (Jason)
        });

        base.Initialize();
    }
}
