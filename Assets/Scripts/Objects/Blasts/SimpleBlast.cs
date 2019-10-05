using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SimpleBlast : PlayerObject
{
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float startRadius = 1;
    [SerializeField]
    private float endRadius = 3;
    [SerializeField]
    private float duration = 2;
    [SerializeField]
    private ConditionData conditionData;

    protected CircleCollider2D coll;
    private List<GameObject> blasted = new List<GameObject>();
    private float endDate;
    private float size;

    protected void Start()
    {
        coll = GetComponent<CircleCollider2D>();
        size = coll.radius;
    }

    public void OnEnable()
    {
        endDate = Time.time + duration;
        blasted.RemoveAll(p => true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.activeSelf) return;

        var go = collision?.gameObject;
        // Debug.Log("trig " + go);
        if (blasted.Find(g => g == go)) return;
                
        var po = collision?.gameObject.GetComponent<PlayerObject>();
        // Debug.Log("collision " + po + " " + po.PlayerId);
        if (po != null && po.PlayerId == PlayerId) return;

        var pl = go.GetComponent<PlayerLife>();
        // Debug.Log("trig2 " + pl);
        if (pl != null) pl.Damage(damage, PlayerId);

        var tl = go.GetComponent<TotemLife>();
        // Debug.Log("trig2 " + pl);
        if (tl != null) tl.Damage(damage, PlayerId);

        var pc = go.GetComponent<PlayerCondition>();
        if (conditionData != null && pc != null)
        {
            pc.AddCondition(conditionData);
        }

        blasted.Add(go);
    }


    protected void Update()
    {
        if(Time.time > endDate) gameObject.SetActive(false);
        var ratio = (endDate - Time.time) / duration;
        coll.radius = size * (startRadius * ratio + endRadius * (1 - ratio));
    }
}