using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTester : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField]
    private BasicTester open;
    [SerializeField]
    private BasicTester ledge;

    void Start()
    {
        playerMovement = transform.parent.gameObject.GetComponent<PlayerMovement>();
        Debug.Assert(playerMovement != null, "could not find player controller");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(open.IsColliding + " " + ledge.IsColliding);
        if (!open.IsColliding && ledge.IsColliding)
            playerMovement.IsGrabbing = true;
        else
            playerMovement.IsGrabbing = false;
    }
}
