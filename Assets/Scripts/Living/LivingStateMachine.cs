using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingStateMachine : StateMachine
{
    public Living living;
    public IdleState IdleState;
    public WalkState WalkState;
    public DigState DigState;

    public StartFlyState StartFlyState;
    public BeingFlyState BeingFlyState;
    public EndFlyState EndFlyState;

    public LivingStateMachine(Living living) : base(living)
    {
        this.living = living;

        this.IdleState = new IdleState(living);
        this.WalkState = new WalkState(living);
        this.DigState = new DigState(living);
        this.StartFlyState = new StartFlyState(living);
        this.BeingFlyState = new BeingFlyState(living);
        this.EndFlyState = new EndFlyState(living);
    }
}
