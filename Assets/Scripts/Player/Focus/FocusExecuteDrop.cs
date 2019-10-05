using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusExecuteDrop : FocusExecute
{
    [SerializeField]
    private GameObject dropPrefab;
    [SerializeField]
    Transform dropTransform;
    [SerializeField]
    bool parentize = false;

    override public void Execute(Vector2 dir)
    {
        var go = Instantiate(dropPrefab);
        go.transform.position = dropTransform.position;
        if(parentize) go.transform.parent = dropTransform.parent;
        var pl = go.GetComponent<PlayerObject>();
        if (pl != null) pl.PlayerId = GetComponent<InputRouter>().PlayerId;
    }
}
