using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleStateMachine : LivingStateMachine
{
    // 상호작용
    public PeopleVendingMachineState PeopleVendingMachineState;

    public PeopleStateMachine(Living people) : base(people)
    {
        this.IdleState = new IdleState(people);
        
        this.PeopleVendingMachineState = new PeopleVendingMachineState(people);
    }
}
