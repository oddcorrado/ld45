using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSimple: MonoBehaviour
{
    public GameObject player;
    public float offsetX=3;
    public float offsetY = 2;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {
        Vector3 position = transform.position;

        if (player == null) return;

        if(player.transform.position.x - transform.position.x < -offsetX)
        {
            position.x = player.transform.position.x + offsetX;

        }
        if(player.transform.position.x - transform.position.x > offsetX)
        {
            position.x = player.transform.position.x - offsetX;
        }

        if (player.transform.position.y - transform.position.y < -offsetY)
        {
            position.y = player.transform.position.y + offsetY;

        }
        if (player.transform.position.y - transform.position.y > offsetY)
        {
            position.y = player.transform.position.y - offsetY;
        }

        transform.position = position;
    }
}
