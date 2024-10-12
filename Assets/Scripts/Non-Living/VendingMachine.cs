using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : NonLiving
{
    protected override void OnEnable()
    {
        base.OnEnable();

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            5
        );
    }

    public override void PlayAnimation(Living targetObjectManager)
    {
        base.PlayAnimation(targetObjectManager);
        
        targetObjectManager.PlayInteractionAnimationByTrigger("InteractToVendingMachine");
    }
}