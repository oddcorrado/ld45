using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerMovement {
    bool GetIsGrounded();
    void SetIsGrounded(bool value);
    bool GetIsWalled();
    void SetIsWalled(bool value);
    bool GetIsStunned();
    void SetIsStunned(bool value);
    void Stun(float duration);
    Collider2D GetWallCollider();
    void SetWallCollider(Collider2D value);
    Vector3 GetNormal();
    void SetNormal(Vector3 value);
}
