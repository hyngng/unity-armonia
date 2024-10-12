using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RenderWidth
{
    public float Left;
    public float Right;
    public float Middle;
    public float Value;
}

public class MainCamera : MonoBehaviour
{
    #region Field
    private Animator animator;
    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    #region Public Methods
    public static RenderWidth GetRenderWidth(GameObject targetObject)
    {
        RenderWidth renderWidth = new RenderWidth();

        renderWidth.Left = Camera.main.ViewportToWorldPoint(new Vector3(0, .5f, targetObject.transform.position.z + Mathf.Abs(Camera.main.transform.position.z))).x;
        renderWidth.Right = Camera.main.ViewportToWorldPoint(new Vector3(1, .5f, targetObject.transform.position.z + Mathf.Abs(Camera.main.transform.position.z))).x;
        renderWidth.Middle = (renderWidth.Right + renderWidth.Left) / 2;
        renderWidth.Value = renderWidth.Right - renderWidth.Left;

        return renderWidth;
    }
    #endregion

    #region Animation Event
    public void EnableRootMotion()
    {
        animator.applyRootMotion = true;
        MainManager.State = Phase.Idle;
    }
    #endregion
}