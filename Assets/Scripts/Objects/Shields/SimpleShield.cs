using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShield : MonoBehaviour
{
    int playerId = -1;
    public enum Action { DESTROY, REFLECT } 
    [SerializeField]
    private Action action = Action.DESTROY;

    void Start()
    {
        var inputRouter = transform?.parent?.gameObject?.GetComponent<InputRouter>();

        if (inputRouter != null) playerId = inputRouter.PlayerId;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        var po = collision?.gameObject.GetComponent<PlayerObject>();

        if (po == null || po.PlayerId == playerId) return;

        switch(action)
        {
            case Action.DESTROY:
                collision?.gameObject.SetActive(false); // TOCHECK
                break;
            case Action.REFLECT:
                var body = collision?.gameObject.GetComponent<Rigidbody2D>();
                if (body != null) body.velocity = -body.velocity;
                if (po != null) po.PlayerId = playerId;
                break;
        }
        
    }
}
