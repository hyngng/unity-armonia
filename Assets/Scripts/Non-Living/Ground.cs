using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : NonLiving
{
    public override void Attract(GameObject targetObject, GameObject accessedObject) // MainManager.cs에서 참조
    {
        Living targetObjectManager = targetObject.GetComponent<Living>();

        targetObjectManager.IsInteracting = true;
        targetObjectManager.InteractingObject = accessedObject;
        targetObjectManager.TargetPosition = targetObject.transform.position;
    }
}