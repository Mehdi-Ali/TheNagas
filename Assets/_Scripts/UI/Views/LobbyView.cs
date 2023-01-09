using System;
using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : View
{
    [SerializeField] private TextMeshProUGUI[] _playerNames = new TextMeshProUGUI[4];

    [SerializeField] private Button _startButton;
    [SerializeField] private Button _readyToggleButton;

    public override void Initialize()
    {

        if (InstanceFinder.IsServer)
        {
            _startButton.onClick.AddListener(() => StartButton());
            _readyToggleButton.gameObject.SetActive(false);
        }

        else
        {
            _readyToggleButton.onClick.AddListener(() => ReadyToggleButton());
            _startButton.gameObject.SetActive(false);
        }

        SetUINicknames();
        
        UpdateView();
        base.Initialize();
    }

    private void ReadyToggleButton()
    {
        Player.LocalPlayer.IsReady = !Player.LocalPlayer.IsReady ;
        // TODO changing the name color to green and red according to ready state
    }

    private void StartButton()
    {
        GameManager.Instance.StartStage();
    }

    private void SetUINicknames()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            _playerNames[i].text = GameManager.Instance.Players[i].PlayerNickName;
        }
    }

    public override void UpdateView()
    {
        base.UpdateView();
        UpdateColors();
        _startButton.interactable = GameManager.Instance.CanStart;
    }

    private void UpdateColors()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            _playerNames[i].color = GameManager.Instance.Players[i].IsReady ? Color.green : Color.red ;
        }
    }
}
