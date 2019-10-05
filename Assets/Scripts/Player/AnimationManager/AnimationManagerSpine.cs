using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SPINE
using Spine.Unity;

public class AnimationManagerSpine : AnimationManager
{
    public override string Movement
    {
        get { return movement; }
        set
        {
            movement = value;
            CheckAnimation();
        }
    }

    public override string ContinuousCommand
    {
        get { return continuousCommand; }
        set
        {
            continuousCommand = value;
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
            // if (command != null) Debug.Log(gameObject.name + " anim " + command);
            if(command != null) animationCoroutine = StartCoroutine(SpineOneShot(command));
        }
    }

    public override string CommandMovement
    {
        get { return commandMovement; }
        set
        {
            if (commandMovement == value) return;
            commandMovement = value;
            // if (command != null) Debug.Log(gameObject.name + " anim " + command);
            if (commandMovement != null) animationCoroutine = StartCoroutine(SpineOneShot(commandMovement));
            else 
            {
                if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            }
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

    private SkeletonAnimation skeletonAnimation;
    private string runningAnimation;
    private Coroutine animationCoroutine;

    private IEnumerator SpineOneShot(string now)
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        // skeletonAnimation.state.ClearTrack(0);
        var track = skeletonAnimation.state.SetAnimation(0, now, false);
        runningAnimation = now;
        yield return new WaitForSpineAnimationComplete(track);
        Command = null;
        CommandMovement = null;
        runningAnimation = null;
        CheckAnimation();
        animationCoroutine = null;
    }

    void Start()
    {
        skeletonAnimation = transform?.Find("Spine").GetComponent<SkeletonAnimation>();
        Debug.Assert(skeletonAnimation != null, "could not find skeletonAnimation");
    }

    private void CheckAnimation()
    {
        if (Command != null) return;

        if (Handicap != null && runningAnimation != Handicap)
        {
            runningAnimation = Handicap;
            // Debug.Log(gameObject.name + " handicap anim " + Handicap);
            skeletonAnimation.state.SetAnimation(0, Handicap, true);
        }
        if (Handicap != null) return;

        if (CommandMovement != null) return;

        if (ContinuousCommand != null && runningAnimation != ContinuousCommand)
        {
            runningAnimation = ContinuousCommand;
            // Debug.Log(gameObject.name + " ContinuousCommand anim " + ContinuousCommand);
            skeletonAnimation.state.SetAnimation(0, ContinuousCommand, true);
        }
        if (ContinuousCommand != null) return;

        if (Movement != null && runningAnimation != Movement)
        {
            runningAnimation = Movement;
            // Debug.Log(gameObject.name + " move anim " + Movement);
            skeletonAnimation.state.SetAnimation(0, Movement, true);
        }
    }

}
#endif