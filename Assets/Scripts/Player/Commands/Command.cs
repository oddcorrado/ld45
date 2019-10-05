using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(InputRouter))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerMana))]
public class Command : MonoBehaviour 
{
    public enum Condition { Grounded, Air}
    [SerializeField]
    protected InputRouter.Key[] keys;
    [SerializeField]
    protected Condition[] conditions;
    [SerializeField]
    protected int cost;
    [SerializeField]
    protected string preAnimation;
    [SerializeField]
    protected string postAnimation;
    [SerializeField]
    protected float cooldown;
    [SerializeField]
    protected ConditionData generalCooldown;
    [SerializeField]
    protected bool cancelable = true;
    [SerializeField]
    protected bool continuous = false;
    [SerializeField]
    protected float loadTime = 0;
    [SerializeField]
    protected PooledBullet PxMana;
    [SerializeField]
    protected PooledBullet PxNoMana;
    [SerializeField]
    protected ConditionData conditionData;

    static public Dictionary<int, Command> runningCommands = new Dictionary<int, Command>();

    protected InputRouter input;
    protected PlayerMovement playerMovement;
    protected PlayerMana playerMana;
    protected bool isLoading = false;
    protected int id = 0;
    private float dateReady = 0;
    private bool prevInputCheck = false;
    private int playerId;
    private AnimationManager animationManager;
    private PlayerCondition playerCondition;
    private PlayerStatusDisplay playerStatusDisplay;
    private float loadStartDate;
    private TintManager tintManager;

    // Use this for initialization
	virtual protected void Start ()
    {
        input = GetComponent<InputRouter>();
        playerId = input.PlayerId;
        playerMovement = GetComponent<PlayerMovement>();
        playerMana = GetComponent<PlayerMana>();
        animationManager = GetComponent<AnimationManager>();
        playerCondition = GetComponent<PlayerCondition>();
        tintManager = GetComponent<TintManager>();
        playerStatusDisplay = transform.Find("StatusCanvas")?.GetComponent<PlayerStatusDisplay>();
        // Debug.Log("playerstatusdisplau"  + playerStatusDisplay);
	}
	
    protected bool CheckConditions()
    {
        foreach(var condition in conditions)
        {
            switch(condition)
            {
                case Condition.Air:
                    if (playerMovement.IsGrounded) return false;
                    break;
                case Condition.Grounded:
                        if (!playerMovement.IsGrounded) return false;
                    break;
                default:
                    break;
            }
        }
        return true;    
    }

    protected bool CheckCost()
    {
        if (playerMana.Mana < cost)
        {
            if(!continuous)
            {
                /* var px = PxNoMana?.Get<PooledBullet>();
                if (px)
                {
                    var position = transform.position;
                    position.z = px.transform.position.z;
                    px.transform.position = position;
                } */
                playerStatusDisplay.ManaMiss = false;
                playerStatusDisplay.ManaMiss = true;
                tintManager.Black(Color.white, 0.1f);
            }
            return false; 
        }
        return true;
    }

    protected bool CheckCooldown()
    {
        if (Time.time < dateReady)
        {
            if(!continuous)
            {
                var px = PxNoMana?.Get<PooledBullet>();
                if (px)
                {
                    var position = transform.position;
                    position.z = px.transform.position.z;
                    px.transform.position = position;
                }  
                tintManager.Black(Color.white, 0.5f);
            }

            return false;
        }
        return true;
    }

    protected bool CheckHandicaps()
    {
        return !playerCondition.Handicaps[(int)PlayerCondition.Handicap.ATTACK];
    }

    protected bool CheckInput()
    {
        var inputCheck = input.Check(keys);
        if (continuous)
        {
            if (inputCheck == false) return false;
        }
        else
        {
            if(loadTime > Mathf.Epsilon)
            {
                // keep this order !
                if (!prevInputCheck && inputCheck) loadStartDate = Time.time;
                prevInputCheck = inputCheck;
                if (inputCheck) 
                {
                    ConditionData condition = new ConditionData()
                    {
                        type = PlayerCondition.Type.LOADING,
                        duration = loadTime,
                        handicaps = new PlayerCondition.Handicap[]
                        {
                            PlayerCondition.Handicap.MOVE,
                            PlayerCondition.Handicap.JUMP
                        },
                        cancels = new PlayerCondition.Type[0]
                    };
                    playerCondition.AddCondition(condition);
                    isLoading = true;
                    return true;
                }
                else
                {
                    if(loadStartDate > 0)
                    {
                        ConditionData condition = new ConditionData()
                        {
                            type = PlayerCondition.Type.LOADING,
                            duration = loadTime,
                            handicaps = new PlayerCondition.Handicap[]
                            {       
                                PlayerCondition.Handicap.MOVE,
                                PlayerCondition.Handicap.JUMP
                            },
                            cancels = new PlayerCondition.Type[0]

                        };
                        playerCondition.RemoveCondition(condition);
                        loadStartDate = 0;
                        isLoading = false;
                    }
                }

            }
            // keep this order !
            if (prevInputCheck && inputCheck) return false;
            prevInputCheck = inputCheck;
            if (inputCheck == false) return false;
        }
        return true;
    }

    protected bool CheckLoadTime()
    {
        if (loadTime < Mathf.Epsilon) return true;

        if (Time.time > loadStartDate + loadTime) 
        {
            isLoading = false;
            return true;
        }

        return false;
    }

    protected bool CheckKeys()
    {
        return input.Check(keys);
    }

    protected bool CheckRunningCommands()
    {
        Command runningCommand;
        runningCommands.TryGetValue(playerId, out runningCommand);
        if (runningCommand != null)
        {
            Debug.Log("running command " + runningCommand);
            if (!runningCommand.cancelable) return false;
            runningCommand.Stop();
            runningCommands.Remove(playerId);
        }
        runningCommands.Add(playerId, this);
        return true;
    }

    protected void ClearRunningCommand()
    {
        Command runningCommand;
        runningCommands.TryGetValue(playerId, out runningCommand);
        if (runningCommand != null)
        {
            runningCommands.Remove(playerId);
            animationManager.Command = null;
        }
            
    }

    protected void UpdateCost()
    {
        playerMana.Mana -= cost;
    }

    protected void UpdateCooldown()
    {
        dateReady = Time.time + cooldown;
        if (generalCooldown != null) playerCondition.AddCondition(generalCooldown);
    }

    public virtual void Stop()
    {
        Debug.LogWarning("Stop not implemented");   
    }

	private void OnDestroy()
	{
        runningCommands.Clear(); // FIXME
	}

	protected bool Check()
    {
        if (CheckInput() == false) return false;
        Debug.Log("CheckInput");
        if (CheckConditions() == false) return false;
        Debug.Log("CheckConditions");
        if (CheckHandicaps() == false) return false;
        Debug.Log("CheckHandicaps");
        if (CheckCost() == false) return false;
        Debug.Log("CheckCost");
        if (CheckCooldown() == false) return false;
        Debug.Log("CheckCooldown");
        if (CheckRunningCommands() == false) return false;
        Debug.Log("CheckRunningCommands");
        if (CheckLoadTime() == false) return false;
        Debug.Log("CheckLoadTime");

        var px = PxMana?.Get<PooledBullet>();
        if (px) px.transform.position = transform.position;

        UpdateCost();
        UpdateCooldown();
        animationManager.Command = preAnimation;
        return true;
    }

}
