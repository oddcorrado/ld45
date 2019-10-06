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

    private Color _startColor;
    private Image _progress;
    private Text _txt;

    private float _value;
    public float Value
    {
        get
        {
            return _value;
        }

        set
        {
            _value = value;
            _value = Mathf.Clamp(_value, 0, 100);
            UpdateValue();
        }
    }
    void Start()
    {
        _progress = transform.Find("Progress").GetComponent<Image>();
        _txt = _progress.transform.Find("Text").GetComponent<Text>();
        _startColor = _progress.color;
        Value = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.KeypadMinus))
        {
            Debug.Log("minus");
            Value--;
        }

        if(Input.GetKey(KeyCode.KeypadPlus))
        {
            Debug.Log("plus");
            Value++;
        }
    }

    private void UpdateValue()
    {
        _txt.text = Value + "%";
        _progress.fillAmount = Value / 100;

        if(Value <= _alert)
        {
            _progress.color = _colorAlert;
        } 
        else
        {
            _progress.color = _startColor;
        }
    }
}
