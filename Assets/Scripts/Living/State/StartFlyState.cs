using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFlyState : IState
{
    private Living living;

    public StartFlyState(Living living)
    {
        this.living = living;
    }

    public void Enter()
    {
        living.Animator.SetTrigger("Fly");
    }

    public void Update()
    {
        Vector3 targetPos = living.IsActivated ?
            new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                living.transform.position.z
            ) :
            living.transform.position; // 일반 비둘기에 대해서 움직일 위치를 지정함
        
        living.transform.position = Vector3.Lerp(
            living.transform.position,
            targetPos,
            5 * Time.deltaTime
        );
        
        // BeingFly로 전환하는 코드는 애니메이션 이벤트로 처리함
            // living.TransitionToBeingFly();
    }

    public void Exit()
    {
        // Debug.Log("Exiting Idle State");
    }
}
