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
    public AiNature myNature;

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
        player = GameObject.Find("BlobSticky").transform;

        var shelters = FindObjectsOfType<Shelter>();
        foreach (var s in shelters) shelterList.Add(s.transform);
    }

    void Update()
    {
        

        var playerScale = Mathf.Abs(player.localScale.x / 30);
        var selfScale = Mathf.Abs(GetComponentInParent<AiMovement>().transform.localScale.x);
        var multiplier = facingLeft ? 1 : -1;

        myText.transform.localScale = new Vector3(
            multiplier * playerScale / selfScale, 
            playerScale / selfScale, 
            playerScale / selfScale);

        switch (myState)
        {
            case AiState.Idle:
                if(SeePlayer()) React();
                else if(Time.time - stateChangeTime > lastCooldownTime)
                {
                    facingLeft = Random.value > 0.5;
                    var speed = facingLeft? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x);

                    SwitchState(speed, 2 + 2 * Random.value, AiState.Walking);
                }
                break;

            case AiState.Walking:
                if (SeePlayer()) React();
                else if (Time.time - stateChangeTime > lastCooldownTime) SwitchState(0, 3 * Random.value, AiState.Idle);
                break;

            case AiState.Fleeing:
                if(Time.time - stateChangeTime > lastCooldownTime)
                {
                    facingLeft = !facingLeft;
                    SwitchState(0, 3 * Random.value, AiState.Idle);
                }
                else jump = !jump;
                break;

            case AiState.SeekingShelter:
                facingLeft = chosenShelter.position.x < transform.position.x;
                horizontalSpeed = facingLeft ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x);

                if (Mathf.Abs(chosenShelter.position.x - transform.position.x) < Mathf.Abs(transform.localScale.x) / 2)
                {
                    horizontalSpeed = 0;
                    if (Mathf.Abs(chosenShelter.position.y - transform.position.y) < Mathf.Abs(transform.localScale.y) / 2)
                        {
                        SwitchState(0, 5 + 5 * Random.value, AiState.Hiding);
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
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                if (Time.time - stateChangeTime > lastCooldownTime)
                {
                    GetComponent<BoxCollider2D>().enabled = true;
                    GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    SwitchState(0, 3 * Random.value, AiState.Idle);
                }
                break;
        }
    }

    private void React()
    {
        switch (myType)
        {
            case AiType.Jumping:
                facingLeft = !facingLeft;
                var speed = facingLeft ? 3 * -Mathf.Abs(transform.localScale.x) : 3 * Mathf.Abs(transform.localScale.x);
                SwitchState(speed, 2 + 2 * Random.value, AiState.Fleeing);
                break;
            case AiType.Hiding:
                if (shelterList != null && shelterList.Count > 0)
                {
                    foreach (var elt in shelterList) if (elt == null) shelterList.Remove(elt);
                    List<Transform> sorted = shelterList.OrderBy(o => Mathf.Abs(o.position.x - transform.position.x)).ToList();
                    chosenShelter = sorted[0];
                }
                facingLeft = chosenShelter.position.x < transform.position.x;
                var speed2 = facingLeft ? 3 * -Mathf.Abs(transform.localScale.x) : 3 * Mathf.Abs(transform.localScale.x);
                SwitchState(speed2, 30, AiState.SeekingShelter);
                break;
        }
    }

    private void SwitchState(float speed, float time, AiState targetState)
    {
        horizontalSpeed = speed;
        lastCooldownTime = time;
        myState = targetState;
    }

    private void UpdateText()
    {
        if (myText != null)
        {
            myText.text = AiDictionnary.dictionnary[(int)myNature, (int)myState][(int)(Random.value * AiDictionnary.dictionnary[(int)myNature, (int)myState].Length)];
        }
    }

    private bool SeePlayer()
    {
        bool directionOk = 
            (facingLeft && player.transform.position.x < transform.position.x && Mathf.Abs(player.transform.position.x - transform.position.x) < Mathf.Abs(transform.localScale.x) * 5) ||
            (!facingLeft && player.transform.position.x > transform.position.x && Mathf.Abs(player.transform.position.x - transform.position.x) < Mathf.Abs(transform.localScale.x) * 5);
        //Vector3 targetDirection = (player.transform.position - transform.position).normalized;
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection);

        //Debug.Log(directionOk + " " + hit.collider.CompareTag("Player"));
        return directionOk;// && hit.collider.CompareTag("Player");
    }
}
