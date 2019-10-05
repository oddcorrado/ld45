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
    private Vector3 _mass = new Vector3(1, 1, 0);
    public Vector3 Mass
    {
        get { return _mass; }
    }
}
