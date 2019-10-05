using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NormalTester : MonoBehaviour
{
    private readonly List<Collider2D> others = new List<Collider2D>();
    private new Collider2D collider;
    private PlayerMovement playerMovement;
    [SerializeField]
    private float maxAngle = 90;

	void Start ()
	{
        playerMovement = transform.parent.gameObject.GetComponent<PlayerMovement>();

        collider = GetComponent<Collider2D>();
        Debug.Assert(collider != null, "could not find player collider");
	}

	private void OnDisable()
	{
        others.RemoveAll(g => true);
	}

	void FixedUpdate ()
    {
        float avg = 0;
        int avgCnt = 0;
        float dirtyShortestDistance = Mathf.Infinity;
        float dirtyShortestAngle = 90;

        string angles = "";
        others.RemoveAll(o => o == null);
        others.ForEach(o =>
        {
            ColliderDistance2D distance2D = o.Distance(collider);
            float angle = Mathf.Rad2Deg * Mathf.Atan2(distance2D.normal.y, distance2D.normal.x);
            Debug.DrawLine(transform.position, transform.position + new Vector3(distance2D.normal.x, distance2D.normal.y, 0), Color.black);
            if (angle < 0) angle += 360;
            if (angle > 180) angle -= 360;
            angles += "/" + angle;
            if (Mathf.Abs(angle - 90) <= maxAngle)
            {
                avg += (angle - 90);
                avgCnt++;

                if (dirtyShortestDistance > distance2D.distance)
                {
                    dirtyShortestAngle = angle - 90;
                    dirtyShortestDistance = distance2D.distance;
                }
            }


        });

        if (avgCnt == 0)
        {
            //Debug.Log(transform.parent.gameObject.name + " no normals: ");
            transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
        // Debug.Log(angles);
        if (avgCnt > 0) avg = avg / avgCnt;

        transform.parent.rotation = Quaternion.Euler(0, 0, dirtyShortestAngle /* avg */);
        if(playerMovement != null)
            playerMovement.Normal = Quaternion.Euler(0, 0, /* avg */ dirtyShortestAngle + 90) * Vector3.right; // FIXME
        //Debug.Log(transform.parent.gameObject.name + " normals: " + avg + "/" + avgCnt);
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
