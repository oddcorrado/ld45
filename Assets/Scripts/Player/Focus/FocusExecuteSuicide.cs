using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusExecuteSuicide : FocusExecute
{
    [SerializeField]
    private int duration = 3;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private FocusSuicideCountdown countdown;

    enum State {IDLE, PRESUICIDE, SUICIDE}
    private State state = State.IDLE;
    private float suicideStart = 0;

    override public void Execute(Vector2 dir)
    {
        suicideStart = Time.time + duration;
        state = State.PRESUICIDE;
        countdown.Display(true);
    }

	void Update()
	{
        if (state != State.PRESUICIDE) return;

        if(Time.time > suicideStart)
        {
            state = State.SUICIDE;
            var go = Instantiate(prefab);
            go.transform.position = transform.position;
            GetComponent<LifeManager>().Life = 0;
            countdown.Display(false);
        }
        else
        {
            countdown.SetCountdown(suicideStart - Time.time);
        }

	}
}
