using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigeon : Living
{
    protected override void Awake()
    {
        base.Awake();

        StateMachine = new PigeonStateMachine(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 상태 패턴
        StateMachine.Initialize(StateMachine.IdleState);

        // 초기 위치 설정하기: 1/4 확률로 날면서 등장함
        bool spawnInFlyState = (Random.Range(0, 4) == 0);
        transform.position = spawnInFlyState
                           ? transform.position
                           : new Vector3(
                                transform.position.x, 
                                Random.Range(InitScale.y, InitScale.y * 5),
                                transform.position.z
                            );
        
        transform.localScale = new Vector3(
            transform.localScale.x * (IsMovingRight ? 1 : -1),
            transform.localScale.y,
            transform.localScale.z
        );

        StartCoroutine(Dig());
    }

    protected override void Update()
    {
        base.Update();

        if (IsInteracting)
            if (Mathf.Approximately(transform.position.x, TargetPosition.x))
                if (StateMachine.CurrentState is IdleState or WalkState)
                    StateMachine.TransitionTo(((PigeonStateMachine)StateMachine).DigState);
    }

    #region Coroutine
    IEnumerator Dig()
    {
        yield return new WaitForSeconds(5);
        
        if (Random.value > 0.5f)
            StateMachine.TransitionTo(StateMachine.DigState);
    }
    #endregion
}