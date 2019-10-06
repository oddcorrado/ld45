using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AiInput : MonoBehaviour
{
    public Text myText;
    public Transform player;
    public AiType myType;

    public List<Transform> shelterList;
    private Transform chosenShelter;

    private AiState state;
    public AiState myState { get => state; set { stateChangeTime = Time.time; state = value; UpdateText(); } }
    private float stateChangeTime;
    private float lastCooldownTime;

    private bool facingLeft = false;

    public float horizontalSpeed;
    public float verticalSpeed;
    public bool jump;
    public bool dive;

    private void Start()
    {
        myState = AiState.Idle;
    }

    void Update()
    {
        switch (myState)
        {
            case AiState.Idle:
                if(SeePlayer())
                {
                    React();
                }
                else if(Time.time - stateChangeTime > lastCooldownTime)
                {
                    facingLeft = Random.value > 0.5;
                    horizontalSpeed = facingLeft? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x);
                    lastCooldownTime = 2 + 2 * Random.value;
                    myState = AiState.Walking;
                }
                break;
            case AiState.Walking:
                if (SeePlayer())
                {
                    React();
                }
                else if (Time.time - stateChangeTime > lastCooldownTime)
                {
                    horizontalSpeed = 0;
                    lastCooldownTime = 3 * Random.value;
                    myState = AiState.Idle;
                }
                break;
            case AiState.Fleeing:
                if(Time.time - stateChangeTime > lastCooldownTime)
                {
                    facingLeft = !facingLeft;
                    horizontalSpeed = 0;
                    lastCooldownTime = 3 * Random.value;
                    myState = AiState.Idle;
                }
                else
                {
                    jump = !jump;
                }
                break;
            case AiState.SeekingShelter:
                facingLeft = chosenShelter.position.x < transform.position.x;
                horizontalSpeed = facingLeft ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x);

                if (Mathf.Abs(chosenShelter.position.x - transform.position.x) < Mathf.Abs(transform.localScale.x) / 2)
                {
                    horizontalSpeed = 0;
                    if (Mathf.Abs(chosenShelter.position.y - transform.position.y) < Mathf.Abs(transform.localScale.y) / 2)
                        {
                        lastCooldownTime = 10 + 10 * Random.value;
                        myState = AiState.Hiding;
                    }
                    else if(chosenShelter.position.y > transform.position.y)
                    {
                        jump = !jump;
                    }
                    else if(chosenShelter.position.y < transform.position.y)
                    {
                        dive = !dive;
                    }
                }
                break;
            case AiState.Hiding:
                break;
        }
    }

    private void React()
    {
        switch (myType)
        {
            case AiType.Jumping:
                facingLeft = !facingLeft;
                horizontalSpeed = facingLeft ? 3 * -Mathf.Abs(transform.localScale.x) : 3 * Mathf.Abs(transform.localScale.x);
                lastCooldownTime = 2 + 2 * Random.value;
                myState = AiState.Fleeing;
                break;
            case AiType.Hiding:
                if (shelterList != null && shelterList.Count > 0)
                {
                    List<Transform> sorted = shelterList.OrderBy(o => Mathf.Abs(o.position.x - transform.position.x)).ToList();
                    chosenShelter = sorted[0];
                }
                myState = AiState.SeekingShelter;
                break;
        }
    }

    private void UpdateText()
    {
        if (myText != null)
        {
            switch (myState)
            {
                case AiState.Idle:
                    myText.text = "I'm idle !";
                    break;
                case AiState.Walking:
                    myText.text = "I'm walking !";
                    break;
                case AiState.Fleeing:
                    myText.text = "I'm fleeing !";
                    break;
                case AiState.SeekingShelter:
                    myText.text = "I'm seeking shelter !";
                    break;
                case AiState.Hiding:
                    myText.text = "I'm hiding !";
                    break;
            }
        }
    }

    private bool SeePlayer()
    {
        bool directionOk = facingLeft && player.transform.position.x < transform.position.x && Mathf.Abs(player.transform.position.x - transform.position.x) < Mathf.Abs(transform.localScale.x) * 5;
        Vector3 targetDirection = (player.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection);
        return directionOk && hit.collider.CompareTag("Player");
    }
}
