using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LandingView : View
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _messageButton;


    [SerializeField] private TMP_InputField _nickNameInput;

    public override void Initialize()
    {
        _hostButton.onClick.AddListener(() =>
        {
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
            SetNickname();
        });

        _connectButton.onClick.AddListener(() =>
        {
            InstanceFinder.ClientManager.StartConnection();
            SetNickname();
        });

        _optionsButton.onClick.AddListener(() =>
            ViewsManager.Instance.Show<OptionsView>());

        _messageButton.onClick.AddListener(() =>
            ViewsManager.Instance.Show<MessageView>("Info..."));

        base.Initialize();
    }

    private void SetNickname()
    {
        SceneDataTransferManager.Instance.PlayerNickName = _nickNameInput.text;
    }

}
