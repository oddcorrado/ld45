using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionData" , menuName = "ScriptableObjects/ConditionData", order = 1)]
public class ConditionData : ScriptableObject
{
    public PlayerCondition.Type type;
    public PlayerCondition.Handicap[] handicaps;
    public float duration = 1;
    public PlayerCondition.Retrigger retrigger;
    public PlayerCondition.Type[] cancels;
    public bool needsReset = false;
    // public int priority;
    // public InputRouter.Key[] reducers;
    // public int damage = 0;
    // public float reduceAmount = 0;
}
