using UnityEngine;
using UnityEngine.UI;

public class OptionsView : View
{
    [SerializeField] private Button _backButton;

    public override void Initialize()
    {
        _backButton.onClick.AddListener(() =>
            ViewsManager.Instance.Show<MultiplayerView>());
        
        // Should make a method in the view manager that goes back to the previous view

        base.Initialize();
    }
}
