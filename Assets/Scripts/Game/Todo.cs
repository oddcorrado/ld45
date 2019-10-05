using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * PRANA : SHOOT OK / SPAWN OK / BITE TODO
 * MAGUS : SHOOT OK / PUNCH OK / SWARM OK
 * DARK ANGEL : SHOOT OK / BLAST OK / REGEN OK
 * WITCH : SHOOT OK / TRAP OK / BLAST OK
 * BRONTU : PUNCH OK / TRAP OK / CHARGE TODO
 * MOGALIN : SHOOT OK / SWITCH 
 * ECTON : KO
 * 
 * JAEGER : PUNCH OK / PUNCH OK / SHOOT OK
 * TESLA : SHOOT OK / TELEPORT Ok / TESLA OK
 * KILLCYCLE : SHOOT OK / BOMB OK / SUICIDE OK
 * RAZORFANE : SHOOT OK / PUNCH OK / TRAP Ok
 * QUICKSILVER : SHOOT OK / PUNCH OK / SHIELD OK
 * WASP : SHOOT OK / HEALTH TODO / FREEZE TODO
 * MANTIS : KO
 *
 * * * * * * * * * * *
 * PUNCH DIRECTIONS
 * PUNCH EXTRA VEL
 * * * * * * * * * * *
 * GFX
 * * * * * * * * * * *
 * FIXES
 * BITE
 * FIX PLAYER CONDITIONS VISUALS
 * TRAPS
 * CHECK DIVE
 * * * * * * * * * * *
 * 
 * CONDITION MANAGER
 * each condition can be triggered by an action
 * each condition has a duration
 * each condition has condiction cancelers (it cancels other conditions)
 * each condition has a cooldown (a kind of immunity timeout to avoid retriggers)
 * each condition is extendable or not (is it extended or not if the action retriggers it)
 * each condition has key accelerators (mashing to exit condition)
 * each condition has handicaps (no input most of the time)
 * the condition can elect a primary condition
 * conditions can be used for combos
 * condition list:
 * BITTEN / FROZEN / CHARGING / DIZZY / RECOVER / COMBO HEAD-FOOT-GUTS
 * 
 * SPECIAL FX IDEAS:
 * CONTROL OTHER PLAYER
 * INVISIBLE
 * CLONE
 * MIRROR
 * ETERNAL EGG
 * ALIEN EGG/BOMB
 */
public class Todo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
