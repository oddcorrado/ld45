using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchZone : MonoBehaviour {
    private CommandPunch commandPunch;
    private List<int> hitIds = new List<int>();

    void Start()
    {
        commandPunch = transform.parent.GetComponent<CommandPunch>();    
    }

	void OnEnable()
	{
        Reset();
	}

	public void Reset()
    {
        hitIds.RemoveAll(h => true);
    }

	void OnTriggerEnter2D(Collider2D collision)
	{
        if(hitIds.FindIndex(id => id == collision.gameObject.GetInstanceID()) == -1)
        {
            hitIds.Add(collision.gameObject.GetInstanceID());
            commandPunch.Hit(collision.gameObject);
        }
	}
}
