using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public enum Type { FROZEN, STUN, HARMLESS, CONFUSED, SICK, HEAVY, CHARGING, LOADING, COOLDOWN }
    public enum Retrigger { IGNORE, OVERWRITE }
    public enum Handicap { MOVE, ATTACK, MANA, INVERT, JUMP, SLOW, WALL } // MANA
    public class Condition
    {
        public Condition(ConditionData conditionData) 
        {
            type = conditionData.type;
            handicaps = conditionData.handicaps;
            duration = conditionData.duration;
            endDate = Time.time + duration;
            retrigger = conditionData.retrigger;
            cancels = conditionData.cancels;
            needsReset = conditionData.needsReset;
        }
        public Type type;
        public Handicap[] handicaps;
        public float duration;
        public float endDate;
        public Retrigger retrigger;
        public int priority;
        public Type[] cancels;
        public bool needsReset;
        // public GameObject originator;
        // public int damage;
        // public InputRouter.Key[] reducers;
        // public float reduceAmount;
    }

    private List<Condition> conditions = new List<Condition>();

    private Condition main;
    private InputRouter inputRouter;
    public Condition Main { get { return main; } }
    private bool[] handicaps = new bool[System.Enum.GetNames(typeof(Handicap)).Length];
    public bool[] Handicaps { get { return handicaps; } set { handicaps = value; } }
    [SerializeField]
    private GameObject[] visualizers = new GameObject[System.Enum.GetNames(typeof(Type)).Length];

    private AnimationManager animationManager;
	void Start()
	{
        animationManager = GetComponent<AnimationManager>();
        inputRouter = GetComponent<InputRouter>();
	}

	void Update()
    {
        conditions.ForEach(c =>
        {
            if(c.endDate < Time.time)
            {
                int vindex = (int)c.type;
                if (vindex < visualizers.Length && visualizers[vindex] != null)
                {
                   visualizers[vindex].SetActive(false);
                }
                // Debug.Log(c.type + " ends");
            }

        });

        conditions.RemoveAll(c => c.endDate < Time.time);
        main = null;
        for (int i = 0; i < Handicaps.Length; i++) Handicaps[i] = false;
        conditions.ForEach(c =>
        {
            foreach(Handicap h in c.handicaps) Handicaps[(int)h] = true;
            if (main == null || c.priority > main.priority) main = c;
        });
    }

    public void RemoveCondition(ConditionData conditionData)
    {
        Condition condition = new Condition(conditionData);
        Condition toRemove = conditions.Find(c => c.type == condition.type);
        if (toRemove != null)
            conditions.Remove(toRemove);
    }

    public void AddCondition(ConditionData conditionData)
    {
        Condition condition = new Condition(conditionData);
        Condition similar = conditions.Find(c => c.type == condition.type);
        if(similar != null)
        {
            if (condition.retrigger == Retrigger.IGNORE) return;
            if (condition.retrigger == Retrigger.OVERWRITE)
            {
                conditions.Remove(similar);
                conditions.Add(condition);
                return;
            }
        }

        foreach(Type type in condition.cancels)
        {
            Condition canceled = conditions.Find(c => c.type == type);
            conditions.Remove(canceled);
            int index = (int)canceled.type;
            if (visualizers[index] != null)
            {
                visualizers[index].SetActive(false);
            }
            Debug.Log(canceled.type + " ends");
        }

        conditions.Add(condition);
        int vindex = (int)condition.type;
        if (vindex < visualizers.Length && visualizers[vindex] != null)
        {
            visualizers[vindex].SetActive(true);
        }
        if (condition.needsReset) inputRouter.NeedsReset = true;
        foreach (Handicap h in condition.handicaps) Handicaps[(int)h] = true;
        // animationManager.CommandMovement = "handicap";
    }
}
