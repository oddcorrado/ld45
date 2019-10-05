using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LifeManager))]
public class CommandFocus : Command
{
    [Serializable]
    public class AngleRange
    {
        public float min;
        public float max;
    }
    [SerializeField]
    private FocusFill focusFill;
    [SerializeField]
    private FocusExecute focusExecute;
    [SerializeField]
    private float duration = 3;
    [SerializeField]
    private Aim aim;
    [SerializeField]
    private AngleRange[] angleRanges;
    [SerializeField]
    private float angleStep = 45;

    enum State { IDLE, FOCUS }
    enum Mode { IDLE, FOCUS }
    private int prevLife;
    private LifeManager lifeManager;
    private State state;
    private float fill = 0;
    private bool prevCheck;
    private Vector2 dir;

    new void Start()
	{
        base.Start();
        lifeManager = GetComponent<LifeManager>();
	}

	private void Reset()
	{
        fill = 0;
        focusFill.SetFill(0);
        focusFill.Display(false);
        aim?.gameObject.SetActive(false);
        state = State.IDLE;
        ClearRunningCommand();
	}

    private Vector2 RestrainAngle()
    {
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        if (angle < 0) angle += 360;

        if (angleRanges.Length == 0)
        {
            angle = (Mathf.Round(angle / angleStep)) * angleStep;
            return Quaternion.Euler(0, 0, angle) * Vector2.right;
        }



        float best = 2000;

        foreach (var range in angleRanges)
        {
            if (angle >= range.min && angle <= range.max) return dir;
            float clamp = Mathf.Clamp(angle, range.min, range.max);

            if (Mathf.Abs(angle - clamp) < Mathf.Abs(angle - best)) best = clamp;
        }

        best = (Mathf.Round(best / angleStep)) * angleStep;
        return Quaternion.Euler(0, 0, best) * Vector2.right;
    }

	void Update()
    {
        bool check = false;
        if (state == State.IDLE)
            check = Check();
        else
            check = CheckInput();
        
        if(check)
            Debug.Log($"focus check {check} state {state}");
        if (state == State.IDLE && check)
        {
            aim?.gameObject.SetActive(true);
            // Debug.Log("dislpqy focus");
            focusFill.Display(true);
            state = State.FOCUS;
            playerMovement.IsLocked = true;
            prevCheck = true;
        }

        if (state == State.IDLE)
        {
            return;
        }

        if(aim != null && state == State.FOCUS)
        {
            var newDir = new Vector2(input.X, input.Y).normalized;
            if (newDir.magnitude > Mathf.Epsilon) dir = newDir;
            dir = RestrainAngle();

            aim.SetAngle(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
        }

        if (lifeManager.Life < prevLife)
        {
            // Debug.Log("Focus Life reset");
            Reset();
            prevLife = lifeManager.Life;
            playerMovement.IsLocked = false;
            return;
        }
        prevLife = lifeManager.Life;

        if (input.J || input.A || input.B || (check && !prevCheck)) // FIXME put in prop
        {
            // Debug.Log($"Focus Move reset {input.J} {input.A} {input.B} {check && !prevCheck} ");
            focusFill.Display(false);
            aim?.gameObject.SetActive(false);
            state = State.IDLE;
            ClearRunningCommand();
            playerMovement.IsLocked = false;
            return;
        }

        prevCheck = check;

        // Debug.Log("FOCUS " + (Time.time - startDate) / duration);
        fill += Time.deltaTime / duration;
        focusFill.SetFill(fill);

        if(fill > 1)
        {
            // state = State.IDLE;
            // startDate = Time.time;
            // Debug.Log("Focus Full reset");
            fill = 0;
            focusExecute.Execute(dir);
            aim?.gameObject.SetActive(false);           
            focusFill.Display(false);
            state = State.IDLE;
            ClearRunningCommand();
            playerMovement.IsLocked = false;
        }
    }
}
