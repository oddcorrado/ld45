using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusExecuteShootDir : FocusExecute
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private float distance = 1;
    [SerializeField]
    private float velocity = 5;

    void Place(GameObject go, Vector2 localDir)
    {
        go.transform.position = transform.position + distance * new Vector3(localDir.x, localDir.y, 0).normalized;
        go.GetComponent<Rigidbody2D>().velocity = localDir * velocity;
#if SPINE
        go.GetComponent<SimpleBullet>().ForceVelocity = localDir * velocity;
#endif
        go.GetComponent<PlayerObject>().PlayerId = GetComponent<InputRouter>().PlayerId;
    }

    override public void Execute(Vector2 dir)
    {
        var go = Instantiate(bullet);
        Place(go, dir);
    }
}
