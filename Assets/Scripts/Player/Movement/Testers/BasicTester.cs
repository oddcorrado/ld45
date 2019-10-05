using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BasicTester : MonoBehaviour
{
    private List<Collider2D> others = new List<Collider2D>();
    public bool IsColliding { get; set; }

    void OnTriggerEnter2D(Collider2D other)
    {
        others.Add(other);
        IsColliding = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        others.Remove(other);
        IsColliding = others.Count > 0;
    }

    private void Update()
    {
        IsColliding = others.Count > 0;
    }
}

