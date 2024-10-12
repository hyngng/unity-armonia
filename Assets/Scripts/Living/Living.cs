using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEditor;

public class Living : MonoBehaviour
{
    #region Field
    [Header("Initial Scale")]
    [Tooltip("1/n배수 ~ n배수를 줌")]
    [Range(1, 1.5f)]
    public float ScaleMultiplier;

    [Header("For those who can fly")]
    public bool CanFly;
    public bool CanHover;

    [Header("Movement")]
    [Range(.5f, 10)]
    [Tooltip("카메라가 얼마나 멀리 떨어져야 오브젝트가 움직이기 시작하는지")]
    public float ApproachThreshold; // WalkState.cs에서 참조
    [Range(0, 1f)]
    public float WalkingSpeed;
    [Range(0, 1f)]
    public float RunningSpeed;
    [HideInInspector]
    public Vector3 TargetPosition;

    [Header("Strong Object Detection")]
    [Range(0, 50)]
    public float DetectRange;
    [Tooltip("프리팹 오브젝트만을 할당할 것")]
    public List<GameObject> TargetObjectList;
    private bool isAvailable;
    public bool IsAvailable
    {
        get { return isAvailable; }
        set
        {
            isAvailable = value;
            if (Animator)
                Animator.SetBool("Available", value);
        }
    }

    [Header("Weak Object Detection")]
    public GameObject HeadPivot;
    public MultiAimConstraint Constraint;
    [Range(0, 10)]
    public float ObjectInteractRange;
    [Tooltip("절차적 애니메이션 이용해 쳐다볼 오브젝트 목록")]
    public List<GameObject> ObjectsToLookAt; // WalkState.cs에서 참조
    private RigBuilder rigBuilder;
    private GameObject objectToLookAt;
    public GameObject ObjectToLookAt
    {
        get { return objectToLookAt; }
        set
        {
            if (Constraint && value != objectToLookAt)
                ChangeSourceObject(value);

            objectToLookAt = value;
        }
    }

    [Header("Audio")]
    public AudioClip WalkingAudioSunny;
    public AudioClip WalkingAudioRainny;
    public AudioClip WalkingAudioSnowy;
    private AudioClip walkingAudio;

    private bool isSelected;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            Selected(value);
            isSelected = value;
        }
    }
    private bool isActivated;
    public bool IsActivated
    {
        get { return isActivated; }
        set
        {
            Activated(value);
            isActivated = value;
        }
    }
    
    [HideInInspector]
    public bool IsMovingRight; // Idle 상태에서 NonLiving 오브젝트와에 의해 상호작용 시 사용됨 + StateMachine, WalkState.cs에서 참조함

    // 기타 변수
    [HideInInspector]
    public bool IsInteracting; // NonLiving.cs, DigState.cs에서 참조
    [HideInInspector]
    public GameObject InteractingObject;
    private float runningTime;
    protected Vector3 prePos, newPos;
    [HideInInspector]
    public Vector3 Velocity; // BeingFlyState.cs, MainCamera.cs에서 참조함
    [HideInInspector]
    public Vector3 InitPos; // WalkState.cs에서 참조함
    [HideInInspector]
    public Vector3 InitScale; // StateMachine, WalkState.cs에서 참조함
    [HideInInspector]
    public Animator Animator; // StateMachine에서 참조함
    public LivingStateMachine StateMachine; //StateMachine에서 참조함
    #endregion

    protected virtual void Awake()
    {
        isSelected = false;
        isActivated = false;
        IsAvailable = false;
        IsInteracting = false;

        TargetPosition = transform.position;
        prePos = transform.position;
        newPos = transform.position;
    }

    protected virtual void OnEnable()
    {
        Animator = GetComponent<Animator>();

        // 이벤트 주도적 프로그래밍
        MainManager.Instance.AddListener(Event.ObjectAccessed, OnEvent);

        // 절차적 애니메이션
        rigBuilder = GetComponent<RigBuilder>();
        
        /* 구상한 코드
        swith (weather)
        {
            case sunny:
                walkingAudio = walkingAudioSunny;
                break;
            case rainy:
                walkingAudio = walkingAudioRainy;
                break;
            case snowy:
                walkingAudio = walkingAudioSnowy;
                break;
        }
        */

        // 오브젝트가 걷는 방향 관련
        IsMovingRight = transform.position.x < MainCamera.GetRenderWidth(gameObject).Middle ? true : false;

        // 오브젝트의 초기 위상 관련
        float scaleMultiplier = UnityEngine.Random.Range(1/ScaleMultiplier, ScaleMultiplier);
        transform.localScale = new Vector3(
            transform.localScale.x * scaleMultiplier,
            transform.localScale.y * scaleMultiplier,
            transform.localScale.z
        );
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y * scaleMultiplier,
            transform.position.z
        );
        InitScale = transform.localScale;

        // 인구수 관련
        ObjectGenerator.Population[this.GetType().Name] += 1;

        InitPos = new Vector3(
            transform.position.x,
            .25f * InitScale.y,
            transform.position.z
        );

        SetObjectAvailability();
        
        StartCoroutine(Move());
    }

    protected virtual void Update()
    {
        // 상태 패턴
        StateMachine.Update();

        bool isOutOfBorder = transform.position.x > MainCamera.GetRenderWidth(gameObject).Right + MainCamera.GetRenderWidth(gameObject).Value
                          || transform.position.x < MainCamera.GetRenderWidth(gameObject).Left - MainCamera.GetRenderWidth(gameObject).Value;
        bool isNotActivatedObject = gameObject != MainManager.ActivatedObject;

        // 카메라로부터 너무 멀리 떨어질 경우 오브젝트 삭제
        if (runningTime > 20f && isOutOfBorder && isNotActivatedObject)
        {
            // 절차적 애니메이션의 SourceObject에서 자신 제거
            Transform[] objectsTransforms = GetChildTransforms(MainManager.ObjectsParent);

            foreach (Transform objectsTransform in objectsTransforms)
            {
                Living targetLiving = objectsTransform.GetComponent<Living>();

                if (targetLiving.ObjectToLookAt && targetLiving.ObjectToLookAt == gameObject)
                    targetLiving.ChangeSourceObject(null);
            }

            Destroy(gameObject);
        }

        // Update문 꼬리 코드
        newPos = transform.position;
        Velocity = (newPos - prePos) / Time.deltaTime;
        prePos = newPos;

        runningTime += Time.deltaTime;
    }

    protected virtual void OnDestroy()
    {
        // 레거시 함수
        ObjectGenerator.Population[this.GetType().Name] -= 1;
        MainManager.Instance.RemoveRedundancies();

        MainManager.Instance.RemoveListener(Event.ObjectAccessed, OnEvent);
    }

    #region Evnet Driven Programming
    public void OnEvent(Event EventType, Component Sender, object Param = null)
    {
        switch (EventType)
        {
            case Event.ObjectAccessed:
                if (Sender == this)
                {
                    IsAvailable = false;

                    switch (MainManager.State)
                    {
                        case Phase.Idle:
                            IsSelected = false;
                            IsActivated = false;
                            break;
                        case Phase.Selected:
                            MainManager.SelectedObject = gameObject;
                            IsSelected = true;
                            break;
                        case Phase.Activated:
                            MainManager.ActivatedObject = gameObject;
                            IsActivated = true;
                            break;
                    }
                }
                else
                {
                    IsSelected = false;
                    IsActivated = false;

                    switch (MainManager.State)
                    {
                        case Phase.Idle: case Phase.Selected:
                            IsAvailable = false;
                            break;
                        case Phase.Activated:
                            SetObjectAvailability();
                            break;
                    }
                }
                break;
        }
    }
    #endregion

    #region Methods
    void SetObjectAvailability()
    {
        if (MainManager.AccessibleObjectList.Count > 0)
        {
            IsAvailable = false;

            List<string> accessibleObjectList = new List<string>();
            foreach (GameObject obj in MainManager.AccessibleObjectList)
            {
                if (gameObject.name.Contains(obj.name))
                {
                    IsAvailable = true;
                    break;
                }
            }
        }
    }

    void Selected(bool value)
    {
        Animator.SetBool("Selected", value);
        
        if (value)
            IsActivated = false;
    }

    void Activated(bool value)
    {
        Animator.SetBool("Activated", value);

        if (value)
            IsSelected = false;
    }

    void Walk()
    {
        if (!IsInteracting)
            StateMachine.TransitionTo(StateMachine.WalkState);
    }

    void Fly()
    {
        if (CanHover)
        {
            /* 곤충들 나는 코드 */
        }
        else
        {
            bool isFlying = StateMachine.CurrentState is StartFlyState || StateMachine.CurrentState is BeingFlyState || StateMachine.CurrentState is EndFlyState;

            if (!isFlying)
                StateMachine.TransitionTo(StateMachine.StartFlyState);
        }
    }
    #endregion

    #region Public Methods
    public void PlayInteractionAnimationByTrigger(string animationName)
    {
        Animator.SetTrigger(animationName);
    }
    #endregion

    #region Animation Event
    public void Vibrate()
    {
        if (MainManager.ActivatedObject == gameObject)
            if (MainManager.IsVibrationOn)
                Vibration.Vibrate((long)2);
    }

    public virtual void StopInteraction()
    {
        InteractingObject = null;
        IsInteracting = false;

        TargetPosition = new Vector3(
            transform.position.x,
            transform.position.y,
            InitPos.z
        );

        if (MainManager.ActivatedObject == gameObject)
            MainCamPositioner.SetCameraPosition(transform.position);

        // 이외에 오버라이딩 필요한 부분이 있음. 자세한 사항은 People.cs에 어떻게 작성되어있는지를 참조.
    }

    public void DiscoverObject() // WalkState.cs에서 참조
    {
        bool hasPassedObject = false;

        Collider closestCollider = Physics.OverlapBox(
            transform.position, 
            new Vector3(ObjectInteractRange, ObjectInteractRange, ObjectInteractRange),
            Quaternion.identity
        ).Where(collider =>
            collider.gameObject.tag == "Living" &&
            collider.gameObject != MainManager.ActivatedObject &&
            ObjectsToLookAt.Any(t => collider.gameObject.name.Contains(t.name))
        ).OrderBy(collider => 
            Vector3.Distance(transform.position, collider.transform.position)
        ).FirstOrDefault();

        if (closestCollider)
            hasPassedObject = IsMovingRight ? transform.position.x - closestCollider.transform.position.x > 0
                                            : closestCollider.transform.position.x - transform.position.x > 0;

        if (hasPassedObject)
            ObjectToLookAt = null;
        else
            ObjectToLookAt = closestCollider ? closestCollider.gameObject : null; 
    }

    public void ChangeSourceObject(GameObject discoveredObject)
    {
        MultiAimConstraintData data = Constraint.data;
        WeightedTransformArray sourceObjects = data.sourceObjects;

        WeightedTransformArray newSourceObjects = new WeightedTransformArray(sourceObjects.Count);
        
        newSourceObjects[0] = new WeightedTransform();
        WeightedTransform wt = newSourceObjects[0];

        if (discoveredObject && discoveredObject.transform != null)
        {
            wt.transform = discoveredObject.transform;
            wt.weight = 0.12f;
        }
        else if (HeadPivot)
        {
            HeadPivot.transform.rotation = Quaternion.identity;
            wt.weight = 0;
        }

        newSourceObjects[0] = wt;

        // 고개 흔드는 방향 설정
        data.aimAxis = IsMovingRight ? MultiAimConstraintData.Axis.Y_NEG : MultiAimConstraintData.Axis.Y;
        
        data.sourceObjects = newSourceObjects;
        Constraint.data = data;

        Animator.enabled = false;
        rigBuilder.Build();
        Animator.enabled = true;
    }

    public void TransitionToBeingFlyState()
    {
        StateMachine.TransitionTo(StateMachine.BeingFlyState);
    }

    public void TransitionToIdleState()
    {
        StateMachine.TransitionTo(StateMachine.IdleState);
    }
    #endregion

    #region Methods
    Transform[] GetChildTransforms(GameObject parent)
    {
        Transform[] childTransforms = new Transform[parent.transform.childCount];

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            childTransforms[i] = parent.transform.GetChild(i);
        }

        return childTransforms;
    }
    #endregion

    #region Coroutine
    IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(1/12);

            bool isFlying = StateMachine.CurrentState is StartFlyState || StateMachine.CurrentState is BeingFlyState || StateMachine.CurrentState is EndFlyState;

            bool notDigging = StateMachine.CurrentState is not DigState;
            bool conditionX = Mathf.Abs(Camera.main.transform.position.x - transform.position.x) >= InitScale.x * 4f;
            bool conditionY = Camera.main.transform.position.y >= InitScale.y * 5f;
            bool shouldFly = CanFly && notDigging && (IsActivated ? (conditionX && conditionY) : transform.position.y > InitScale.y);

            if (isFlying || shouldFly)
                Fly();
            else if (StateMachine.CurrentState is not DigState)
                Walk();
        }
    }
    #endregion
}