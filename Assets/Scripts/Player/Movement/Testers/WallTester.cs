using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WallTester : MonoBehaviour 
{
    private PlayerMovement playerMovement;
    private List<Collider2D> others = new List<Collider2D>();
    private PlayerCondition playerCondition;
    [SerializeField]
    private PooledBullet pxPrefab;

    void Start ()
    {
        playerMovement = transform.parent.gameObject.GetComponent<PlayerMovement>();
        playerCondition = transform.parent.gameObject.GetComponent<PlayerCondition>();
        Debug.Assert(playerMovement != null, "could not find player controller");
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        others.Add(other);
        playerMovement.WallCollider = other;
        var px = pxPrefab.Get<PooledBullet>(true);
        px.transform.position = transform.position;
        px.transform.rotation = transform.rotation;
        playerMovement.IsWalled = true;
    }

    void OnTriggerExit2D (Collider2D other)
    {
        others.Remove(other);
        playerMovement.IsWalled = others.Count > 0;
        playerMovement.WallCollider = others.Count > 0 ? others[0] : null;
    }

    private void Update()
    {
        // string sum = " SUM ";
        // others.RemoveAll(o => o == null);
        //others.ForEach(o => sum += " " + o.gameObject.name);
        if (playerCondition.Handicaps[(int)PlayerCondition.Handicap.WALL])
        {
            playerMovement.IsWalled = false;
            playerMovement.WallCollider = null;
            return;
        }
        playerMovement.IsWalled = others.Count > 0;
        playerMovement.WallCollider = others.Count > 0 ? others[0] : null;
        // Debug.Log(others.Count);
    }
}

