using System;
using System.Collections.Generic;
using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : View
{
    [SerializeField] private List<TextMeshProUGUI> _playerNames = new();

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

        base.Initialize();
    }

    public override void UpdateView()
    {
        base.UpdateView();

        UpdateNicknames();
        
        UpdateColors();

        _startButton.interactable = GameManager.Instance.CanStart;
    }

    private void ReadyToggleButton()
    {
        Player.LocalPlayer.SetIsReadyStatus(!Player.LocalPlayer.IsReady);
    }

    private void StartButton()
    {
        GameManager.Instance.ServerStartStage();
    }

    private void UpdateNicknames()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            _playerNames[i].text = GameManager.Instance.Players[i].PlayerNickName;
        }
        
    }

    private void UpdateColors()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            _playerNames[i].color = GameManager.Instance.Players[i].IsReady ? Color.green : Color.red ;
        }
    }

}
