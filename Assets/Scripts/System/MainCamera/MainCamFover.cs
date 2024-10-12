using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamFover : MonoBehaviour
{
    #region Fields
    [Header("Settings")]
    public float ZoomLerpSpeed;

    [Header("FOV Standard")]
    [Tooltip("Box Collider가 부착되어 있어야 함")]
    public GameObject StandardObject;
    public float SettingsFovMultiplier;

    private Camera cameraComponent;
    private float fovStandard; // 기준 오브젝트의 크기를 1로 잡아야 하므로, 분모가 됨.
    private float initFOV;
    public static float TargetFOV; // TouchManager.cs에서 참조
    public static float FlyingOffset;
    #endregion

    void Awake()
    {
        FlyingOffset = 1;
    }

    void Start()
    {
        Vector2 StandardObjectSize = StandardObject.GetComponent<BoxCollider>().size;
        fovStandard = StandardObjectSize.x * StandardObjectSize.y;
        
        cameraComponent = GetComponent<Camera>();
        initFOV = cameraComponent.fieldOfView;
        TargetFOV = cameraComponent.fieldOfView;
    }

    void Update()
    {
        float objectSize = GetObjectSize();

        switch (MainManager.State)
        {
            default:
                HandleDefaultPhase(objectSize);
                break;
            case Phase.Selected:
                HandleSelectedPhase(objectSize);
                break;
        }

        cameraComponent.fieldOfView = Mathf.Lerp(
            cameraComponent.fieldOfView,
            TargetFOV,
            Time.deltaTime * ZoomLerpSpeed
        );
    }

    private float GetObjectSize()
    {
        Vector2 colliderSize = Vector2.one;
        float objectScale = 1;

        if (MainManager.SelectedObject || MainManager.ActivatedObject)
        {
            if (MainManager.ActivatedObject)
                colliderSize = MainManager.ActivatedObject.GetComponent<BoxCollider>().size;
            else if (MainManager.SelectedObject)
                colliderSize = MainManager.SelectedObject.GetComponent<BoxCollider>().size;

            objectScale = MainManager.ActivatedObject
                        ? MainManager.ActivatedObject.transform.localScale.x
                        : MainManager.SelectedObject.transform.localScale.x;
        }
        
        // 코드 좀 더러운 편.
        return Mathf.Abs(objectScale) * ((colliderSize == Vector2.one) ? (fovStandard) : (colliderSize.x * colliderSize.y));
    }

    void HandleDefaultPhase(float objectSize)
    {
        if (MainManager.IsInSettings)
            TargetFOV = initFOV * (objectSize / fovStandard) * SettingsFovMultiplier;
        else
        {
            if (Input.touchCount == 2)
                TargetFOV = Mathf.Clamp(TargetFOV + TouchManager.PinchDelta * .75f, initFOV * .8f * (objectSize / fovStandard), initFOV * 1.25f * (objectSize / fovStandard));
            else
                TargetFOV = initFOV * FlyingOffset * (objectSize / fovStandard);
        }
    }

    void HandleSelectedPhase(float objectSize)
    {
        TargetFOV = initFOV * .8f * (objectSize / fovStandard);
    }
}
