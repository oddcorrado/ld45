using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerSizeDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject rabbits;

    private PlayerMovementGroundSticky script;
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        script = rabbits.GetComponent<PlayerMovementGroundSticky>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        DisplaySize();
    }

    private void DisplaySize()
    {
        float result = Mathf.Pow(script.SizeMul, 1.3f) / 274f;

        if (result < 0.01)
        {
            // Go to mm
            result *= 1000;
            text.text = Math.Truncate(result) + " mm";
        }
        else if (result < 0.1)
        {
            // Go to cm
            result *= 100;
            text.text = Math.Truncate(result) + " cm";
        }
        else if (result < 1)
        {
            // Go to dm
            result *= 10;
            text.text = Math.Truncate(result) + " dm";
        }
        else
        {
            // Go to m
            text.text = Math.Truncate(result) + " m";
        }
    }
}
