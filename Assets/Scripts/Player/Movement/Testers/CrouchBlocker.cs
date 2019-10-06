using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CrouchBlocker : MonoBehaviour
{
    private readonly List<Collider2D> others = new List<Collider2D>();
    private new Collider2D collider;

    public bool Block { get; set; }

    void Start ()
    {
        collider = GetComponent<Collider2D>();
        Debug.Assert(collider != null, "could not find player collider");
    }

    private void OnDisable()
    {
        others.RemoveAll(g => true);
    }

    private float GetAngle(float avg, float shortest)
    {
        return shortest;
    }

    void FixedUpdate ()
    {
        others.RemoveAll(o => o == null);
        Block = others.Count > 1;

        // Debug.Log((Block ? "BLOCK" : "CROUCH OK"));
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        others.Add(other);
    }

    void OnTriggerExit2D (Collider2D other)
    {
        others.Remove(other);
    }
}
