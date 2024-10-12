using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeingFlyState : IState
{
    # region Fields
    private Living living;
    private float currentVerticalMovementAmount;
    public float TargetVerticalMovementAmount;
    # endregion

    public BeingFlyState(Living living)
    {
        this.living = living;
    }

    public void Enter()
    {
        if (living.IsActivated)
        {
            MainCamPositioner.IsTrackingActivatedObject = true;
            MainCamFover.FlyingOffset = 1.2f;
        }
    }

    public void Update()
    {
        // 위치 전환
        float speed = living.InitScale.x * 25f;

        TargetVerticalMovementAmount = living.IsActivated ? TargetVerticalMovementAmount : 0;
        currentVerticalMovementAmount = Mathf.Clamp(
            Mathf.Lerp(
                currentVerticalMovementAmount,
                TargetVerticalMovementAmount,
                Time.deltaTime * 6
            ),
            -1,
            .5f
        );

        living.transform.Translate(
            new Vector3(living.IsMovingRight ? 1 : -1, 0, 0) * speed * Time.deltaTime
            + (living.CanHover ? Vector3.zero : new Vector3(0, (living.InitScale.y + currentVerticalMovementAmount * 5f) * -.1f, 0))
        );

        // 방향 전환
        /* living.IsMovingRight = living.transform.position.x < MainCamera.GetRenderWidth(living.gameObject).Middle ? true : false;

        living.transform.localScale = new Vector3(
            living.InitScale.x * (living.IsMovingRight ? 1 : -1),
            living.transform.localScale.y,
            living.transform.localScale.z
        );
        */

        // 애니메이션
        float livingVelocity = Mathf.Abs(living.Velocity.x);
        living.Animator.SetFloat("Speed", Mathf.Clamp(livingVelocity, 1, livingVelocity) * .125f);

        // EndFlyState로 전환
        if (living.transform.position.y <= living.InitScale.y)
            living.StateMachine.TransitionTo(living.StateMachine.EndFlyState);
    }

    public void Exit()
    {
        living.Animator.SetTrigger("Land");
    }
}
