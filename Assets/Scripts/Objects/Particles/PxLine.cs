using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PxLine : MonoBehaviour
{
    [SerializeField]
    private Vector2 startPos = Vector2.zero;
    public Vector2 StartPos
    {
        get { return startPos; }
        set { startPos = value;  }
    }

    [SerializeField]
    private Vector2 endPos = Vector2.zero;
    public Vector2 EndPos
    {
        get { return endPos; }
        set { endPos = value; UpdatePos(); }
    }

    private float z = -10;
    public float Z
    {
        get { return z; }
        set { z = value; UpdatePos(); }
    }

	private void Update()
	{
        UpdatePos();
	}
	void UpdatePos()
    {
        var pos = 0.5f * (StartPos + EndPos);
        transform.position = new Vector3(pos.x, pos.y, Z);

        var scale = (StartPos - EndPos).magnitude;
        transform.localScale = new Vector3(scale, 1, 1);

        var angle = Mathf.Rad2Deg * Mathf.Atan2(EndPos.y - StartPos.y, EndPos.x - StartPos.x);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
