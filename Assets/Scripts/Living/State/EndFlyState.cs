using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndFlyState : IState
{
    private Living living;

    public EndFlyState(Living living)
    {
        this.living = living;
    }

    public void Enter()
    {
        if (living.IsActivated)
        {
            MainCamPositioner.IsTrackingActivatedObject = false;
            MainCamFover.FlyingOffset = 1;
        }  
    }

    public void Update()
    {
        // IdleState로 전환하는 코드는 애니메이션 이벤트로 처리함
            // living.TransitionToBeingFly();
    }

    public void Exit()
    {
        living.transform.position = new Vector3(
            living.transform.position.x,
            living.InitScale.y,
            living.transform.position.z
        );
    }
}
