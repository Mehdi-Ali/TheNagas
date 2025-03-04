using System.Net.Mime;
using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro _text ;
    private Color _textColor ;
    [SerializeField] private Vector3 _moveVector = new Vector3(0.7f * 60f , 0f, 60f);
    [SerializeField] private float _maxDisappearTime = 1.0f;
    [SerializeField] private float _disappearSpeed = 5.0f;
    [SerializeField] private float _increaseScaleAmount = 1.0f;
    [SerializeField] private float _decreaseScaleAmount = 2.0f;


    //Utilities
    private float _disappearTime ;
    private PopupsParent _popupsParent ;


    private static int _sortingOrder ;
    public static DamagePopUp Create(Vector3 position, float damage, Color color)
    {         
        var instance = Instantiate(GameAssets.i.DamagePopUp, position, Quaternion.identity) ;
        var damagePopUpInstance = instance.GetComponent<DamagePopUp>();
        damagePopUpInstance.SetUp(damage, color);

        return damagePopUpInstance ;
    }

    void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _popupsParent = FindObjectOfType<PopupsParent>();
    }

    public void SetUp(float damage, Color color)
    {
        // set parent to , _popupsParent.transform
        this.transform.SetParent(_popupsParent.transform);
        _text.SetText(damage.ToString());
        _text.color = color;
        _disappearTime = _maxDisappearTime ;
        _text.sortingOrder = _sortingOrder ;
        _sortingOrder ++ ;
    }

  
    void Update()
    {
        _disappearTime -= Time.deltaTime;
        _moveVector -= _moveVector * 8.0f * Time.deltaTime ;

        if (_disappearTime > _maxDisappearTime *0.5f)
        {
            transform.position += _moveVector * Time.deltaTime;
            transform.localScale += Vector3.one * _increaseScaleAmount * Time.deltaTime ;
        }

        else if (_disappearTime < _maxDisappearTime * 0.5f)
        {
            transform.position -= _moveVector * Time.deltaTime;
            transform.localScale -= Vector3.one * _decreaseScaleAmount * Time.deltaTime ;
        }

        if (_disappearTime > 0 )
        {
            _textColor = _text.color ;
            _textColor.a -= _disappearSpeed * Time.deltaTime ;
            _text.color = _textColor ;

            //TODO make ipt pool
            if (_textColor.a <= 0) Destroy(this.gameObject);
        }
    }
}
