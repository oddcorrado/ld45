using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BlobProgressBarEat : MonoBehaviour
{
    [SerializeField]
    private Color _colorAlert = Color.red;

    [SerializeField]
    private float _alert = 25f;

    [SerializeField]
    private AnimationCurve _curveDecreaseFood;

    [SerializeField]
    private float _decreaseRate = 1f;    

    private Color _startColor;
    private Image _progress;
    private Image _background;
    private Text _txt;
    private GameObject blobUI;
    private MainMenu _gameover;
    private Coroutine flashCoroutine;

    [SerializeField]
    private int _valueProgressBarMax = 100;
    public int ValueProgressBarMax
    {
        get { return _valueProgressBarMax; }
        set { _valueProgressBarMax = value; }
    }

    private float _value;
    public float Value
    {
        get
        {
            return _value;
        }

        set
        {
            if(value > _value)
            {
                if (flashCoroutine != null) StopCoroutine(flashCoroutine);
                flashCoroutine = StartCoroutine(Flash());
            }
            _value = value;
            _value = Mathf.Clamp(_value, 0, _valueProgressBarMax);
            UpdateValue();
        }
    }
    private void Start()
    {
        _progress = transform.Find("Progress").GetComponent<Image>();
        _background = transform.Find("Background").GetComponent<Image>();
//         _txt = _progress.transform.Find("Text").GetComponent<Text>();
        _startColor = _progress.color;
        Value = 100;

        blobUI = GameObject.Find("BlobUI");
        if(blobUI != null)
            _gameover = blobUI.GetComponent<MainMenu>();        

        StartCoroutine(DecreaseHunger());
    }

    private void UpdateValue()
    {
      //   _txt.text = (int)((Value / _valueProgressBarMax) * 100) + "%";
        _progress.fillAmount = Value / _valueProgressBarMax;

        if(Value <= _alert)
        {
            _progress.color = _colorAlert;
        } 
        else
        {
            _progress.color = _startColor;
        }

    }

    private IEnumerator Flash()
    {
        _background.color = Color.red;
        while(_background.color.r > 0)
        {
            _background.color = new Color(_background.color.r - 0.05f, 0, 0);
            yield return new WaitForSeconds(0.05f);
            Debug.Log(_background.color);
        }

        _background.color = new Color(0, 0, 0, 0.5f);
        flashCoroutine = null;
    }

    private IEnumerator DecreaseHunger()
    {
        while(Value > 0)
        {
            Value -= _curveDecreaseFood.Evaluate(_valueProgressBarMax);
            yield return new WaitForSeconds(_decreaseRate);
        }

        if (Value <= 0)
            _gameover.GameOver();
    }
}