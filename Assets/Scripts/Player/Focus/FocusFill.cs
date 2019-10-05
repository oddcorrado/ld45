using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusFill : MonoBehaviour
{
    [SerializeField]
    private Image image;

    public void SetFill(float fill)
    {
        image.fillAmount = fill;
    }

    public void Display(bool show)
    {
        transform.parent.gameObject.SetActive(show);
    }

	void Update()
	{
        if (transform.parent.parent.localScale.x + 1 < Mathf.Epsilon)
            transform.parent.localScale = new Vector2(-1, 1);
        else
            transform.parent.localScale = new Vector2(1, 1);
	}
}
