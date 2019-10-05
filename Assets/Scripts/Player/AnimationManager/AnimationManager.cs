using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    protected string movement = null;
    public virtual string Movement
    {
        get { return movement; }
        set
        {
//            Debug.Log("AnimationManager " + value);
            movement = value;
        }
    }

    protected string command = null;
    public virtual string Command
    {
        get { return command; }
        set
        {
  //          Debug.Log("AnimationManager " + value);
            if (command == value) return;
            command = value;
        }
    }

    protected string continuousCommand = null;
    public virtual string ContinuousCommand
    {
        get { return continuousCommand; }
        set
        {
            continuousCommand = value;
            // CheckAnimation();
        }
    }

    protected string commandMovement = null;
    public virtual string CommandMovement
    {
        get { return commandMovement; }
        set
        {
            if (commandMovement == value) return;
            commandMovement = value;
            // if (command != null) Debug.Log(gameObject.name + " anim " + command);
        }
    }

    protected string handicap = null;
    public virtual string Handicap
    {
        get { return handicap; }
        set
        {
            Debug.Log("AnimationManager " + value);
            handicap = value;
        }
    }
}
