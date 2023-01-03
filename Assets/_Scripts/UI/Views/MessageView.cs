using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class MessageView : View
{
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _backButton;

    public override void Initialize()
    {
        _backButton.onClick.AddListener(() =>
            ViewsManager.Instance.Show<LandingView>());

        base.Initialize();
    }

    public override void Show(object args = null)
    {
        var message = "No Data";

        if (args is string messageReceived)
            message = messageReceived;

        _messageText.text = message;

        base.Show(args);
    }
}
