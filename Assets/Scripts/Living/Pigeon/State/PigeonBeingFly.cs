using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonBeingFly : IState
{
    private Living pigeon;

    public PigeonBeingFly(Living pigeon)
    {
        this.pigeon = pigeon;
    }

    public void Enter()
    {
        // Debug.Log("Entering VendingMachine State");
    }

    public void Update()
    {
        // Debug.Log("Currently In VendingMachine State");
    }

    public void Exit()
    {
        // Debug.Log("Exiting VendingMachine State");
    }
}