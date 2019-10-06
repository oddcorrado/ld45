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

    void DestroyObject(GameObject go)
    {
        prefab.Get<PooledBullet>();
        prefab.transform.position = go.transform.position;
        prefab.transform.localScale = Vector3.one * playerMovementGroundSticky.SizeMul;
        Destroy(go); 
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.bounds.size.x < playerMovementGroundSticky.SizeMul) DestroyObject(other.gameObject);
        if (other.bounds.size.y < playerMovementGroundSticky.SizeMul) DestroyObject(other.gameObject);
    }
}
