using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorAnimationManager : AnimationManager
{
    [SerializeField]
    private List<string> nameToId;
    [SerializeField]
    private string prefix;

    public override string Movement
    {
        get { return movement; }
        set
        {
            movement = value;
            CheckAnimation();
        }
    }

    public override string Command
    {
        get { return command; }
        set
        {
            if (command == value) return;
            command = value;
            if (command != null) animationCoroutine = StartCoroutine(OneShot(command));
        }
    }

    public override string Handicap
    {
        get { return handicap; }
        set
        {
            handicap = value;
            CheckAnimation();
        }
    }

    private Animator animator;
    private string runningAnimation;
    private Coroutine animationCoroutine;

    private IEnumerator OneShot(string now)
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        // skeletonAnimation.state.ClearTrack(0);
        runningAnimation = now;
        int id = nameToId.FindIndex(n => n == now);
        if (id == -1) id = 0;
        animator.SetInteger("animationId", id);
        runningAnimation = now;
        Debug.Log("waiting for " + now);
        while(!animator.GetCurrentAnimatorStateInfo(0).IsName(now)) {
            yield return new WaitForEndOfFrame();
        }
        Command = null;
        runningAnimation = null;
        CheckAnimation();
        animationCoroutine = null;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "could not find animator");
        Debug.Log("OK");
    }

    private void CheckAnimation()
    {
        if (Handicap != null && runningAnimation != Handicap)
        {
            runningAnimation = Handicap;
            int id = nameToId.FindIndex(n => n == Handicap);
            // Debug.Log(gameObject.name + " anim " + Handicap + " " + id);
            if (id == -1) id = 0;
            animator.SetInteger("animationId", id);
            return;

        }

        if (Handicap != null) return;

        if (Command != null) return;

        if (Movement != null && runningAnimation != Movement)
        {
            runningAnimation = Movement;
            int id = nameToId.FindIndex(n => n == Movement);
            if (id == -1) id = 0;
            // Debug.Log(">" + Movement + " id=" + id);
            animator.SetInteger("animationId", id);
        }
    }

}
