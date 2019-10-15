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

    public float horizontalSpeed;
    public float verticalSpeed;
    public bool jump;
    public bool dive;
    public float jumpiness = 0.5f;
    PlayerMovementGroundSticky playerMovementGroundSticky;
    PreyStats preyStats;

    private void Start()
    {
        myState = AiState.Idle;
        player = GameObject.Find("BlobSticky").transform;
        playerMovementGroundSticky = player.GetComponent<PlayerMovementGroundSticky>();
        preyStats = GetComponent<PreyStats>();

        var shelters = FindObjectsOfType<Shelter>();
        foreach (var s in shelters) shelterList.Add(s.transform);
    }

    void Update()
    {

        var playerScale = Mathf.Abs(player.localScale.x / 26);
        var selfScale = Mathf.Abs(GetComponentInParent<AiMovement>().transform.localScale.x);
        var multiplier = Mathf.Sign(GetComponentInParent<AiMovement>().transform.localScale.x);

        myText.transform.localScale = new Vector3(
            multiplier * playerScale / selfScale, 
            playerScale / selfScale, 
            1);

        switch (myState)
        {
            case AiState.Idle:
                if (SeePlayer())
                {
                    React();
                }
                else if (Time.time - stateChangeTime > lastCooldownTime)
                {
                    horizontalSpeed = Random.value > 0.5 ? -1 : 1;
                    SwitchState(1 + 2 * Random.value, AiState.Walking);
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
                    SwitchState(1 + 1 * Random.value, AiState.Idle);
                }
                break;

            case AiState.Fleeing:

                if (SeePlayer())
                {
                    horizontalSpeed = player.transform.position.x < transform.position.x ? 1 : -1;
                    // jump = !jump;
                    if (Random.value < jumpiness) jump = true;
                    else jump = false;
                }
                else
                {
                    horizontalSpeed = 0;
                    SwitchState(1 + 3 * Random.value, AiState.Idle);
                }
                break;

            case AiState.SeekingShelter:
                horizontalSpeed = chosenShelter.position.x < transform.position.x ? -1 : 1; //-Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x);

                if (Mathf.Abs(chosenShelter.position.x - transform.position.x) < Mathf.Abs(transform.localScale.x) / 2)
                {
                    horizontalSpeed = 0;
                    if (Mathf.Abs(chosenShelter.position.y - transform.position.y) < Mathf.Abs(transform.localScale.y) / 2)
                        {
                        SwitchState(5 + 5 * Random.value, AiState.Hiding);
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
                    SwitchState(3 * Random.value, AiState.Idle);
                }
                break;
        }
    }

    private void React()
    {
        switch (myType)
        {
            case AiType.Jumping:
                SwitchState(2 + 2 * Random.value, AiState.Fleeing);
                break;
            case AiType.Hiding:
                if (shelterList != null && shelterList.Count > 0)
                {
                    foreach (var elt in shelterList) if (elt == null) shelterList.Remove(elt);
                    List<Transform> sorted = shelterList.OrderBy(o => Mathf.Abs(o.position.x - transform.position.x)).ToList();
                    chosenShelter = sorted[0];
                }
                SwitchState(30, AiState.SeekingShelter);
                break;
        }
    }

    private void SwitchState(float time, AiState targetState)
    {
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
        // if (playerMovementGroundSticky.SizeMul < preyStats.SizeMulMin) return false;
        bool directionOk = Mathf.Abs(player.transform.position.x - transform.position.x) < Mathf.Abs(transform.localScale.x) * 10;
           // (facingLeft && player.transform.position.x < transform.position.x && Mathf.Abs(player.transform.position.x - transform.position.x) < Mathf.Abs(transform.localScale.x) * 10) ||
            // (!facingLeft && player.transform.position.x > transform.position.x && Mathf.Abs(player.transform.position.x - transform.position.x) < Mathf.Abs(transform.localScale.x) * 10);

        //Vector3 targetDirection = (player.transform.position - transform.position).normalized;
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection);

        //Debug.Log(directionOk + " " + hit.collider.CompareTag("Player"));
        return directionOk;// && hit.collider.CompareTag("Player");
    }
}
