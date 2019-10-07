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
    [SerializeField]
    private bool useAverage = false;

    private float rootMaxAngle;
    public int SurfaceCount { get; set; }

	void Start ()
	{
        playerMovement = transform.parent.gameObject.GetComponent<PlayerMovement>();

        collider = GetComponent<Collider2D>();
        Debug.Assert(collider != null, "could not find player collider");
        rootMaxAngle = maxAngle;
	}

	private void OnDisable()
	{
        others.RemoveAll(g => true);
	}

    private float GetAngle(float avg, float shortest)
    {
        return useAverage ? avg : shortest;
    }

    public void MakeSticky(bool isSticky)
    {
        maxAngle = isSticky ? 360 : rootMaxAngle;
        useAverage = isSticky;
    }

	void Update ()
    {
        float avg = 0;
        float dirtyShortestDistance = Mathf.Infinity;
        float dirtyShortestAngle = 90;
        SurfaceCount = 0;
        // Debug.Log(transform.parent.name + "fck " + maxAngle);

        string angles = "";
        others.RemoveAll(o => o == null);
        others.ForEach(o =>
        {
            ColliderDistance2D distance2D = o.Distance(collider);
            float angle = Mathf.Rad2Deg * Mathf.Atan2(distance2D.normal.y, distance2D.normal.x);
            Debug.DrawLine(transform.position, transform.position + new Vector3(distance2D.normal.x, distance2D.normal.y, 0), Color.black);
            if (angle < 0) angle += 360;
            // if (angle > 180) angle -= 360;
            // CHECK THESE LINES
            if (avg - angle > 180) angle += 360;
            if (avg - angle < -180) angle -= 360;
            // CHECK

            if (Mathf.Abs(angle - 90) <= maxAngle)
            {
                angles += "/[*" + angle + " " + o.name + "]";
                avg += (angle - 90);
                SurfaceCount++;

                if (dirtyShortestDistance > distance2D.distance)
                {
                    dirtyShortestAngle = angle - 90;
                    dirtyShortestDistance = distance2D.distance;
                }
            }
            else
                angles += "/[*" + angle + " " + o.name + "*" + Mathf.Abs(angle - 90) + "vs" + maxAngle + "]";


        });

        if (SurfaceCount == 0)
        {
            // Debug.Log(transform.parent.gameObject.name + " no normals: ");
            transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            if(playerMovement != null) playerMovement.Normal = Vector3.zero;
            return;
        }

        if (SurfaceCount > 0) avg = avg / SurfaceCount;

        // Debug.Log(angles + " " + avg + " " + SurfaceCount + " " +(GetAngle(avg, dirtyShortestAngle) + 90));

        transform.parent.rotation = Quaternion.Euler(0, 0, GetAngle(avg, dirtyShortestAngle));
        if(playerMovement != null)
            playerMovement.Normal = Quaternion.Euler(0, 0, GetAngle(avg, dirtyShortestAngle) + 90) * Vector3.right; // FIXME
        // Debug.Log(angles + " " + avg + " " + (GetAngle(avg, dirtyShortestAngle) + 90) + playerMovement.Normal);
        // Debug.Log(transform.parent.gameObject.name + " normals: " + (avg + 90) + "/" + SurfaceCount + "/" + playerMovement.Normal);
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
