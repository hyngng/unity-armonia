using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateMachine
{
    public IState CurrentState { get; private set; }
    public event Action<IState> stateChanged;

    //public WalkState walkState; 처럼 상태 변수 정의

    public StateMachine(Living living)
    {
        // this.idleState = new IdleState(livingObject); 처럼 내용 정의
    }

    public void Initialize(IState state)
    {
        CurrentState = state;
        state.Enter();

        stateChanged?.Invoke(state);
    }

    // exit this state and enter another
    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();

        stateChanged?.Invoke(nextState);
    }

    public void Update()
    {
        if (CurrentState != null)
            CurrentState.Update();

        // Debug.Log($"{gameObject.transform.position.x}은 현재 {CurrentState} + 임");
    }
}