using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonStateMachine : LivingStateMachine
{
    // 상호작용
    // public VendingMachineState VendingMachineState;

    public PigeonStateMachine(Living pigeon) : base(pigeon)
    {
        this.IdleState = new IdleState(pigeon);
        
        // this.VendingMachineState = new VendingMachineState(people);
    }
}
