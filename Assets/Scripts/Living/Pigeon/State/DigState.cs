using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigState : IState
{
    private Living living;

    public DigState(Living living)
    {
        this.living = living;
    }

    public void Enter()
    {
        living.Animator.SetTrigger("Dig");
    }

    public void Update()
    {
        // Debug.Log("Currently In VendingMachine State");
    }

    public void Exit()
    {
        living.IsInteracting = false;
    }
}