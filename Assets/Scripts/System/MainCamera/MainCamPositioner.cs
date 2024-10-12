using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamPositioner : MonoBehaviour
{
    #region Fields
    [Header("Settings")]
    public float Intensity;
    public float MoveLerpSpeed;
    [Range(.5f, 1.5f)]
    [Tooltip("선택한 오브젝트로부터 떨어질 수 있는 최대 화면비율상 거리")]
    public float DistanceLimit;

    private MainCamPositioner positioner;
    private MainCamFover fover;

    private Vector3 initPosition;
    public static Vector3 TargetPosition; // TouchManager.cs에서 참조
    public static bool IsTrackingActivatedObject; // BeingflyState.cs, EndFlyState에서 참조

    #endregion

    void Awake()
    {
        IsTrackingActivatedObject = false;
    }

    void Start()
    {
        initPosition = transform.position;
        TargetPosition = transform.position;

        positioner = GetComponent<MainCamPositioner>();
        fover = GetComponent<MainCamFover>();
    }

    void Update()
    {
        if (!MainManager.IsInSettings)
        {
            switch (MainManager.State)
            {
                case Phase.Idle:
                    HandleIdlePhase();
                    break;
                case Phase.Selected:
                    HandleSelectedPhase();
                    break;
                case Phase.Activated:
                    HandleActivatedPhase();
                    break;
            }

            if (MainManager.State == Phase.Activated)
            { 
                float positionStandard = (MainCamera.GetRenderWidth(MainManager.ActivatedObject).Right - MainCamera.GetRenderWidth(MainManager.ActivatedObject).Left) * 0.5f;
                float distanceFromObject = Mathf.Abs(transform.position.x - MainManager.ActivatedObject.transform.position.x);

                if (distanceFromObject > positionStandard * DistanceLimit)
                {
                    float targetX = MainManager.ActivatedObject.transform.position.x + (MainManager.ActivatedObject.transform.position.x > transform.position.x ? positionStandard : -positionStandard) * DistanceLimit;
                    TargetPosition = Vector3.Lerp(TargetPosition, new Vector3(targetX, TargetPosition.y, initPosition.z), Time.deltaTime);
                }

                if (!MainManager.ActivatedObjectManager.IsInteracting)
                {
                    Vector3 activatedObjectPosition = MainManager.ActivatedObject.transform.position;
                    MainManager.ActivatedObjectManager.TargetPosition = new Vector3(transform.position.x, activatedObjectPosition.y, activatedObjectPosition.z);
                }
            }
        }

        transform.position = Vector3.Lerp(
            transform.position,
            TargetPosition,
            Time.deltaTime * MoveLerpSpeed
        );
    }

    #region Methods
    void HandleIdlePhase()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.touches[0];
            TargetPosition -= new Vector3(
                touch.deltaPosition.x * Intensity,
                touch.deltaPosition.y * Intensity,
                0
            );
        }
    }

    void HandleSelectedPhase()
    {
        TargetPosition = new Vector3(
            MainManager.SelectedObject.transform.position.x,
            MainManager.SelectedObject.transform.position.y,
            initPosition.z
        );
    }

    void HandleActivatedPhase()
    {
        if (IsTrackingActivatedObject)
            HandleActivatedObjectTracking();
        else
            HandleActivatedObjectMovement();
    }

    void HandleActivatedObjectTracking()
    {
        if (Input.touchCount == 0)
        {
            MainManager.ActivatedObjectManager.StateMachine.BeingFlyState.TargetVerticalMovementAmount = 0;
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.touches[0];
            MainManager.ActivatedObjectManager.StateMachine.BeingFlyState.TargetVerticalMovementAmount = touch.deltaPosition.y * Intensity;
        }

        TargetPosition = new Vector3(
            MainManager.ActivatedObject.transform.position.x,
            MainManager.ActivatedObject.transform.position.y,
            initPosition.z
        );
    }

    void HandleActivatedObjectMovement()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.touches[0];
            TargetPosition -= new Vector3(
                touch.deltaPosition.x * Intensity,
                touch.deltaPosition.y * Intensity,
                0
            );
        }
    }
    #endregion

    #region Public Methods
    public static void SetCameraPosition(Vector3 newPosition) // Living.cs에서 참조
    {
        TargetPosition = new Vector3(
            newPosition.x,
            TargetPosition.y,
            TargetPosition.z
        );
    }
    #endregion
}
