using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayView : View
{
    // Make this class content an interface IHasInfo
    // to inherent from in all the views that i Want to have info On
    // or add a SuperView that get ignored when hiding views

    [SerializeField] private Button _DisconnectButton;
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _playersCountText;

    public override void Initialize()
    {
        _DisconnectButton.onClick.AddListener(() =>
            InstanceFinder.ClientManager.StopConnection());

        base.Initialize();
    }
    
    private void LateUpdate()
    {
        if (!IsInitialized)
            return;

        _infoText.text = $"Is Server = {InstanceFinder.IsServer}, Is Client = {InstanceFinder.IsClient}."; 
        
        // should be updated on change rather in updates.
        _scoreText.text = $"Score: {Player.Instance.Score}";

        _playersCountText.text = $"Players Connected: {InstanceFinder.ServerManager.Clients.Count}.";
    }
}
