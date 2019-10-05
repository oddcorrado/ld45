using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusSuicideCountdown : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public void SetCountdown(float secs)
    {
        text.text = secs.ToString("N1");
    }

    public void Display(bool show)
    {
        transform.parent.gameObject.SetActive(show);
    }
}
