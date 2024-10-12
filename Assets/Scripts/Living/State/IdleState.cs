using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private Living living;

    public IdleState(Living living)
    {
        this.living = living;
    }

    public void Enter()
    {
        // Debug.Log("Entering Idle State");
    }

    public void Update()
    {
        // Debug.Log("Currently In Idle State");
    }

    public void Exit()
    {
        // Debug.Log("Exiting Idle State");
    }
}