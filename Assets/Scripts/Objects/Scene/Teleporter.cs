using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField]
    Teleporter next;

    private List<GameObject> skips = new List<GameObject>();
    public List<GameObject> Skips { get { return skips; } set { skips = value; } }

	void OnTriggerEnter2D(Collider2D collision)
	{
        var otherGo = collision.gameObject;
        int otherId = otherGo.GetInstanceID();
        // Debug.Break();

        if (Skips.FindIndex(go => go == otherGo) != -1) return;
        if (otherGo.GetComponent<InputRouter>() == null) return;
        Debug.Log(name + "enter " + otherGo.name + " " + otherId);
        next.Skips.Add(otherGo);
        otherGo.transform.position = next.transform.position;
	}

	void OnTriggerExit2D(Collider2D collision)
	{
        var otherGo = collision.gameObject;
        int otherId = otherGo.GetInstanceID();
        Debug.Log(name + "exit " + otherGo.name + " " + otherId);
        Skips.Remove(otherGo);
	}
}
