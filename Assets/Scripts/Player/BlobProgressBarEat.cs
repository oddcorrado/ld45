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
    private float _decreaseFood = 1f;

    [SerializeField]
    private float _decreaseRate = 1f;    

    private Color _startColor;
    private Image _progress;
    private Text _txt;

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
            _value = value;
            _value = Mathf.Clamp(_value, 0, _valueProgressBarMax);
            UpdateValue();
        }
    }
    private void Start()
    {
        _progress = transform.Find("Progress").GetComponent<Image>();
        _txt = _progress.transform.Find("Text").GetComponent<Text>();
        _startColor = _progress.color;
        Value = 100;

        StartCoroutine(DecreaseHunger());
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private IEnumerator DecreaseHunger()
    {
        while(Value > 0)
        {
            Value -= _decreaseFood;
            yield return new WaitForSeconds(_decreaseRate);
        }

        // TODO : game over
    }
}