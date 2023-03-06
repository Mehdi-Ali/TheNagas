using System.Net.Mime;
using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using UnityEngine.InputSystem.OnScreen;
using FishNet.Managing.Scened;

public class MoveJoyStickSnap : MonoBehaviour
{
    /*
    TODO: 
    the name of the script should be changed To TouchFunctionHandler, or even better separate
    the class to AbilityCancel and JoyStickSnap and a TouchManager Class that handle both of
    the classes and make it scalable to add other touche function...  
    */

    [SerializeField]
    private RectTransform _joyStick;
    [SerializeField]public RectTransform CancelAbilityRectTrans;
    public PlayerStateManger Player;
    private Image _image ;
    private Vector2 _sizeOffset ;
    private Vector2 _screenDim;
    private float _xLimit;
    private float _yLimit;
    private float _xPos;
    private float _yPos;
    private SceneManager sceneManager;

    private float _cancelX;
    private float _cancelY;
    private Vector2 _CancelSizeOffset;
    private float _cancelXX;
    private float _cancelYY;

    public void Awake()
    {
        _joyStick = GetComponent<RectTransform>();
        sceneManager = StageManager.Instance.SceneManager;
        sceneManager.OnLoadEnd += OnLoadStage;

        _sizeOffset = _joyStick.sizeDelta / 2f;
        _screenDim = new Vector2(Screen.width, Screen.height);
        _image = GetComponentInChildren<Image>();

        _xLimit = 0.35f * _screenDim.x;
        _yLimit = 0.75f * _screenDim.y;

        _xPos = 0.15f * _screenDim.x;
        _yPos = 0.18f * _screenDim.y;

        _cancelX = CancelAbilityRectTrans.position.x;
        _cancelY = CancelAbilityRectTrans.position.y;
        _CancelSizeOffset = CancelAbilityRectTrans.sizeDelta;
        _cancelXX = _cancelX - _CancelSizeOffset.x;
        _cancelYY = _cancelY + _CancelSizeOffset.y;
    }

    public void OnDestroy()
    {
        sceneManager.OnLoadEnd -= OnLoadStage;
    }
    
    public void OnStartClient()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += FingerUp;
    }

    public void OnStopNetwork()
    {
        EnhancedTouchSupport.Disable();
        TouchSimulation.Disable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= FingerUp;
    }

    public void OnLoadStage(SceneLoadEndEventArgs obj)
    {
        _joyStick = GetComponent<RectTransform>();
    }

    private static void GetScreenPosition(Finger finger, out float xAxis, out float yAxis)
    {
        var screenPosition = finger.screenPosition;
        xAxis = screenPosition.x;
        yAxis = screenPosition.y;
    }

    void FingerDown(Finger finger)
    {
        float xAxis, yAxis;
        GetScreenPosition(finger, out xAxis, out yAxis);

        if (xAxis < 10f || xAxis > _xLimit || yAxis < 10f || yAxis > _yLimit)
            return;

        var xPos = xAxis - _sizeOffset.x;
        var yPos = yAxis - _sizeOffset.y;

        if (_joyStick != null)
            _joyStick.position = new Vector3(xPos, yPos, 0f);

        if (_image != null)
            _image.enabled = true ;
    }
    
    private void FingerUp(Finger finger)
    {
        float xAxis, yAxis;
        GetScreenPosition(finger, out xAxis, out yAxis);

        if (_image != null && xAxis < (_xLimit * 1.5f) && yAxis < _yLimit)
            _image.enabled = false ;;

        if (xAxis < _cancelX && xAxis > _cancelXX && yAxis < _cancelYY && yAxis > _cancelY)
            Player.CancelAbility();

    }

}
