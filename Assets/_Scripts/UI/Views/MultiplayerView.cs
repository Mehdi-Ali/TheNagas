using FishNet;
using UnityEngine;
using UnityEngine.UI;

public sealed class MultiplayerView : View
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _messageButton;

    public override void Initialize()
    {
        _hostButton.onClick.AddListener(() =>
        {
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        });

        _connectButton.onClick.AddListener(() =>
            InstanceFinder.ClientManager.StartConnection());

        _optionsButton.onClick.AddListener(() =>
            ViewsManager.Instance.Show<OptionsView>());

        _messageButton.onClick.AddListener(() =>
            ViewsManager.Instance.Show<MessageView>("Info..."));

        base.Initialize();
    }
}
