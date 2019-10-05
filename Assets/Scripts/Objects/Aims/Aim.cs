using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
	public void SetAngle(float angle)
	{
        transform.rotation = Quaternion.Euler(0, 0, angle);
	}
}
