using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyStats : MonoBehaviour
{
    [SerializeField]
    private string _name = "test";
    public string Name
    {
        get { return _name; }
    }

    [SerializeField]
    private float _sizeMulMin = 1;
    public float SizeMulMin
    {
        get { return _sizeMulMin; }
    }
}
