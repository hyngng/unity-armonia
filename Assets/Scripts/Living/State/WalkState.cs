using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WalkState : IState
{
    private Living living;

    public WalkState(Living living)
    {
        this.living = living;
    }

    public void Enter()
    {
        // Debug.Log("Entering VendingMachine State");
    }

    public void Update()
    {
        Vector3 targetPositionWithInitPos = new Vector3(living.TargetPosition.x, living.TargetPosition.y, living.InitPos.z);
        Vector3 distanceFromTarget = (living.IsInteracting ? living.TargetPosition : targetPositionWithInitPos) - living.transform.position;
        Vector3 direction = (living.TargetPosition - living.transform.position).normalized;

        // 이거 작동안함. 로직 수정해야함. (오브젝트가 Walk는 하는데 Run은 안함)
        float moveSpeed = living.IsActivated ? Mathf.Clamp(
            (living.RunningSpeed * living.InitScale.x * distanceFromTarget.magnitude / (MainCamera.GetRenderWidth(living.gameObject).Value / 4)),
            living.WalkingSpeed * living.InitScale.x,
            living.RunningSpeed * living.InitScale.x
        ) : living.WalkingSpeed;

        if (living.IsActivated)
        {
            // 조종되고 있을 때
            if (living.IsInteracting)
            {
                // 상호작용 중에
                if (Mathf.Approximately(living.transform.position.x, living.TargetPosition.x))
                    living.Animator.SetTrigger("Stop");
                else
                {
                    living.transform.Translate(
                        Mathf.Abs(distanceFromTarget.x) < Mathf.Abs(moveSpeed * direction.x * Time.deltaTime)
                        ? new Vector3(distanceFromTarget.x, 0, distanceFromTarget.z)    
                        : moveSpeed * direction * Time.deltaTime
                    );

                    living.Animator.SetBool("Run", false);
                    living.Animator.SetTrigger("Walk");
                }
            }
            else
            {
                // 카메라 따라 이동 중에
                if (Mathf.Abs(distanceFromTarget.x) > (MainCamera.GetRenderWidth(living.gameObject).Value / 4))
                {
                    // 달리기
                    living.transform.Translate(moveSpeed * direction);
                    living.Animator.SetBool("Run", true);
                }
                else if (Mathf.Abs(distanceFromTarget.x) > living.ApproachThreshold)
                {
                    // 걷기
                    living.transform.Translate(moveSpeed * direction);
                    living.Animator.SetBool("Run", false);
                    living.Animator.SetTrigger("Walk");
                }
                else
                    living.Animator.SetTrigger("Stop");
            }

            if (direction.x > 0) 
                living.IsMovingRight = true;
            else if (direction.x < 0)
                living.IsMovingRight = false;
        }
        else
        {
            // 조종되고있지 않을 때
            if (living.IsInteracting)
            {
                // 상호작용 중에
                if (Mathf.Approximately(living.transform.position.x, living.TargetPosition.x))
                    living.Animator.SetTrigger("Stop");
                else
                {
                    living.transform.Translate(
                        Mathf.Abs(distanceFromTarget.x) < Mathf.Abs(moveSpeed * direction.x)
                        ? new Vector3(distanceFromTarget.x, 0, distanceFromTarget.z)    
                        : moveSpeed * direction
                    );

                    living.Animator.SetBool("Run", false);
                    living.Animator.SetTrigger("Walk");
                }
            }
            else
            {
                living.transform.Translate(moveSpeed * (living.IsMovingRight ? 1 : -1), 0, 0);
                living.Animator.SetTrigger("Walk");
            }
        }

        living.Animator.SetFloat("Speed", Mathf.Clamp(Mathf.Abs(moveSpeed * direction.x) / moveSpeed, .5f, 1));

        living.transform.localScale = new Vector3(
            living.InitScale.x * (living.IsMovingRight ? 1 : -1),
            living.transform.localScale.y,
            living.transform.localScale.z
        );

        // 리깅 애니메이션
        if (living.Constraint)
            living.DiscoverObject();
    }

    public void Exit()
    {
        // Debug.Log("Exiting VendingMachine State");
        // living.ObjectToLookAt = null;
    }
}
