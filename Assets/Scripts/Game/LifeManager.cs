using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    protected int life;
    public virtual int Life
    {
        get { return life; }
        set
        {
            life = value;
        }
    }

    void Start()
    {
    }

    public virtual void Damage(int value, int playerId)
    {
    }

    public virtual void Cure(int value, int playerId)
    {
    }
}
