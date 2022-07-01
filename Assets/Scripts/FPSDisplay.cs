using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI FPSText;
    //[SerializeField] int _targetFrameRate = -1 ;

    private float _pollingTime = 0.25f;
    private float _time = 0.0f ;
    private int _frameCount = 0 ;
    private int _frameRate = 0 ;

    void Start()
    {
       // Application.targetFrameRate = _targetFrameRate;
    }

    void Update()
    {
        _time += Time.deltaTime ;

        _frameCount ++;

        if ( _time >= _pollingTime )
        {
            _frameRate = Mathf.RoundToInt(_frameCount / _time );
            FPSText.text = _frameRate.ToString() + " FPS" ;

            _time -= _pollingTime ;
            _frameCount = 0 ;
        }    
    }
}
