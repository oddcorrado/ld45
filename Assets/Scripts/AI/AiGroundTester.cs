using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AiGroundTester : MonoBehaviour
{
    private readonly List<Collider2D> others = new List<Collider2D>();
    private AiMovementBase playerMovement;
    [SerializeField]
    private PooledBullet pxPrefab;

    private Collider2D parentCollider;
    private Collider2D wallCollider;
    private BoxCollider2D groundCollider;

    void Start()
    {
        playerMovement = transform.parent.gameObject.GetComponent<AiMovementBase>();
        Debug.Assert(playerMovement != null, "could not find player controller");

        wallCollider = transform.parent.Find("WallTester")?.gameObject.GetComponent<Collider2D>();
        // Debug.Assert(wallCollider != null, transform.parent.Find("WallTester") + " could not find wallCollider");

        parentCollider = transform.parent.GetComponent<Collider2D>();
        groundCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        others.Add(other);

        if (!playerMovement.IsWalled && !playerMovement.IsGrounded)
        {
            var px = pxPrefab.Get<PooledBullet>(true);
            var position = transform.position;
            position.z = px.transform.position.z;
            px.transform.position = position;
            px.transform.rotation = transform.rotation;
            playerMovement.AirCommandCount = 0;
        }
        playerMovement.IsGrounded = others.Count > 0;
        ResizeCollider();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Break();
        others.Remove(other);
        playerMovement.IsGrounded = others.Count > 0;
        if (!playerMovement.IsGrounded)
        {
            var px = pxPrefab.Get<PooledBullet>(true);
            var position = transform.position;
            position.z = px.transform.position.z;
            px.transform.position = position;
            px.transform.rotation = transform.rotation;
        }
        ResizeCollider();
    }

    void ResizeCollider()
    {
        // Debug.Log(transform.parent.name + " " + playerMovement.IsGrounded);
        if (!playerMovement.IsGrounded)
        {
            groundCollider.offset = new Vector2(0, -0.2f);
            groundCollider.size = new Vector2(0.5f, 0.5f);

        }
        else
        {
            groundCollider.offset = new Vector2(0, 0);
            groundCollider.size = new Vector2(0.5f, 0.1f);
        }
    }
    IEnumerator TraverseCoroutine()
    {
        Collider2D[] toIgnore = others.ToArray();

        foreach (Collider2D other in toIgnore)
        {
            if (other.gameObject.layer != 15)
            {
                Physics2D.IgnoreCollision(other, parentCollider, true);
                if (wallCollider != null)
                    Physics2D.IgnoreCollision(other, wallCollider, true);
            }
        }
        yield return new WaitForSeconds(0.5f);

        foreach (Collider2D other in toIgnore)
        {
            if (other.gameObject.layer != 15)
            {
                Physics2D.IgnoreCollision(other, parentCollider, false);
                if (wallCollider != null)
                    Physics2D.IgnoreCollision(other, wallCollider, false);
            }
        }
    }
    public void Traverse()
    {
        bool cancel = false;

        others.ForEach(o =>
        {
            if (o.gameObject.layer != 17) cancel = true;
        });
        if (cancel) return;
        StartCoroutine(TraverseCoroutine());
    }

    private void Update()
    {
        string sum = "";
        others.RemoveAll(o => o == null);
        others.ForEach(o => sum += " " + o.gameObject.name);
        playerMovement.IsGrounded = others.Count > 0;
        // Debug.Log(others.Count + sum);
    }
}
