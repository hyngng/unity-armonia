using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleVendingMachineState : IState
{
    private Living people;

    public PeopleVendingMachineState(Living people)
    {
        this.people = people;
    }

    public void Enter()
    {
        people.InteractingObject.GetComponent<NonLiving>().PlayAnimation(people);
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