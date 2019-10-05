using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDropTrigger : CommandDrop
{
    [SerializeField]
    protected InputRouter.Key[] triggerKeys;
    [SerializeField]
    protected bool continuousTrigger;

    private bool prevInputCheckTriggers;

    protected bool CheckTrigger()
    {
        var inputCheck = input.Check(triggerKeys);
        if (continuousTrigger)
        {
            if (inputCheck == false) return false;
        }
        else
        {
            // keep this order !
            if (prevInputCheckTriggers && inputCheck) return false;
            prevInputCheckTriggers = inputCheck;
            if (inputCheck == false) return false;
        }
        return true;
    }

    protected override void Update()
    {
        base.Update();

        if (!CheckTrigger()) return;

        objects.ForEach(o =>
        {
            if(o.activeSelf)
            {
                var t = o.GetComponent<TrapTrigger>();
                t?.Trigger();
            }
        });
    }
}
