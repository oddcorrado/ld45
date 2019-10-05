using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : PlayerObject
{
    // Update is called once per frame
    public virtual void Trigger()
    {
        Debug.Log("TRIGGER");
    }
}
