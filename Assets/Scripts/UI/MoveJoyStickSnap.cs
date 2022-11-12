using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;


public class MoveJoyStickSnap : MonoBehaviour
{
    private RectTransform _joyStick;
    [SerializeField] private RectTransform _cancelAbility;
    private Vector2 _sizeOffset ;
    private Vector2 _screenDim;
    private float _xLimit;
    private float _yLimit;
    private float _xPos;
    private float _yPos;

    void Awake()
    {
        _joyStick = GetComponent<RectTransform>();
        _sizeOffset = _joyStick.sizeDelta / 2f;
        _screenDim = new Vector2(Screen.width, Screen.height);

        _xLimit = 0.35f * _screenDim.x;
        _yLimit = 0.75f * _screenDim.y;

        _xPos = 0.15f * _screenDim.x;
        _yPos = 0.18f * _screenDim.y;
    }
    
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += FingerUp;

    }


    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        TouchSimulation.Disable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= FingerUp;
    }

    void FingerDown(Finger finger)
    {
        var screenPosition = finger.screenPosition;
        var xAxis = screenPosition.x;
        var yAxis = screenPosition.y;

        if (xAxis < 10f || xAxis > _xLimit || yAxis < 10f || yAxis > _yLimit)
            return;

        var xPos = xAxis - _sizeOffset.x;
        var yPos = yAxis - _sizeOffset.y;

        _joyStick.position = new Vector3(xPos, yPos, 0f);
    }
    
    private void FingerUp(Finger finger)
    {
        var screenPosition = finger.screenPosition;
        var xAxis = screenPosition.x;
        var yAxis = screenPosition.y;

        if (xAxis < _xLimit && yAxis < _yLimit)
            _joyStick.position = new Vector3(_xPos, _yPos, 0f);


        // ----


        var cancelX = _cancelAbility.position.x ;
        var cancelY = _cancelAbility.position.y ;

        var CancelSizeOffset = _cancelAbility.sizeDelta;

        var cancelXX = cancelX - CancelSizeOffset.x ; // small
        var cancelYY = cancelY + CancelSizeOffset.y ; //big

        if (xAxis < cancelX && xAxis > cancelXX && yAxis < cancelYY && yAxis > cancelY)
            Debug.Log("Cancel");
        //Debug.Log("click " + screenPosition);

        // Debug.Log("x " + cancelX);
        // Debug.Log("xx " + cancelXX);
        // Debug.Log("y " + cancelY);
        // Debug.Log("yy " + cancelYY);

    }
}
