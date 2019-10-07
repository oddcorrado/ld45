using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobEvolution : MonoBehaviour
{
    private BlobEat blobEat;

    public enum FoodType { FLEA, ANT, MICE, RABBIT, CHAMELEON, HUMAN }
    public enum Feature { JUMP, STICKY, DOUBLE_JUMP, CONCEAL }

    [System.Serializable]
    public class Food
    {
        public FoodType type;
        public AnimationCurve sizeContribution = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(10, 1) });
        public Feature featureUnlock;
    }

    [SerializeField] float groundSpeedStart = 8;
    [SerializeField] float jumpGroundVelStart = 14;
    [SerializeField] List<Food> foods = new List<Food>();

    private PlayerMovementGroundSticky playerMovementGroundSticky;

    void Start()
    {
        blobEat = GetComponent<BlobEat>();
        playerMovementGroundSticky = GetComponent<PlayerMovementGroundSticky>();
    }

    // Update is called once per frame
    void Update()
    {
        float size = 1;
        float speed = groundSpeedStart;
        float jump = jumpGroundVelStart;

        foreach (string key in blobEat.Preys.Keys)
        {
            int count = blobEat.Preys[key];
            Food food = foods.Find(f => f.type.ToString() == key);
            if(food != null)
            {
                size += food.sizeContribution.Evaluate(count);
                speed += food.sizeContribution.Evaluate(count);
                jump += food.sizeContribution.Evaluate(count);
            }
            // Debug.Log(key + food.featureUnlock);
            playerMovementGroundSticky.SizeMul = size * 0.1f + playerMovementGroundSticky.SizeMul * 0.9f;
            if (food.featureUnlock == Feature.STICKY && !playerMovementGroundSticky.IsSticky) playerMovementGroundSticky.IsSticky = true;
        }
    }
}
