using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDestroyer : MonoBehaviour
{
    private PlayerMovementGroundSticky playerMovementGroundSticky;
    [SerializeField] PooledBullet prefab;

    void Start()
    {
        playerMovementGroundSticky = transform.parent.gameObject.GetComponent<PlayerMovementGroundSticky>();
    }

    void DestroyObject(GameObject go, Bounds bounds)
    {
        Debug.Log("destroy " + go.name + go.transform.position);
        var px = prefab.Get<PooledBullet>();
        px.transform.position = go.transform.position;
        px.transform.localScale = new Vector3(bounds.size.x, bounds.size.y, 1);
        Destroy(go); 
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.bounds.size.x < playerMovementGroundSticky.SizeMul) DestroyObject(other.gameObject, other.bounds);
        if (other.bounds.size.y < playerMovementGroundSticky.SizeMul) DestroyObject(other.gameObject, other.bounds);
    }
}
