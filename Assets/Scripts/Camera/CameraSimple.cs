using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSimple: MonoBehaviour
{
    public GameObject player;
    public float offsetX=3;
    public float offsetY = 2;
    private Camera cam;
    private float f = 1;
    PlayerMovementGroundSticky playerMovementGroundSticky;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        playerMovementGroundSticky = player.GetComponent<PlayerMovementGroundSticky>();
    }

    // Update is called once per frame

    void Update()   
    {
        Vector3 position = transform.position;
        f = 1.5f + playerMovementGroundSticky.SizeMul;

        if (player == null) return;

        if(player.transform.position.x - transform.position.x < -offsetX * f)
        {
            position.x = player.transform.position.x + offsetX * f;

        }
        if(player.transform.position.x - transform.position.x > offsetX * f)
        {
            position.x = player.transform.position.x - offsetX * f;
        }   

        if (player.transform.position.y - transform.position.y < -offsetY * f)
        {
            position.y = player.transform.position.y + offsetY * f;

        }
        if (player.transform.position.y - transform.position.y > offsetY * f)
        {
            position.y = player.transform.position.y - offsetY * f;
        }

        transform.position = position;

        cam.orthographicSize = 3 * Mathf.Pow(f, 1.3f);
    }
}
