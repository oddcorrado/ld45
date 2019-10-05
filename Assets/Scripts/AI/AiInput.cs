using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiInput : MonoBehaviour
{
    public Transform player;

    private AiState state;
    public AiState myState { get => state; set { stateChangeTime = Time.time; state = value; } }
    private float stateChangeTime;

    public float horizontalSpeed;
    public float verticalSpeed;
    public bool jump;

    private void Start()
    {
        myState = AiState.Idle;
    }

    void Update()
    {
        switch (myState)
        {
            case AiState.Idle:
                myState = AiState.Walking;
                break;
            case AiState.Walking:
                if (Time.time % 10 > 5) horizontalSpeed = -1;
                else horizontalSpeed = 1;
                break;
            case AiState.Fleeing:
                break;
        }
    }
}
