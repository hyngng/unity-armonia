using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonLiving : MonoBehaviour
{
    #region Fields
    [Header("Properties")]
    [Range(0.05f, 1)]
    [Tooltip("오브젝트가 최종적으로 상호작용하는 거리")]
    public float InteractDistance;
    [Tooltip("오브젝트가 감지되는 최대 거리, 단위는 초(second)")]
    public float InteractBoxSize;
    public float ObjectAttractCycle;
    public bool OnlyVisibleInSettings;

    [Header("Features")]
    [Tooltip("오브젝트의 개성")]
    public List<Sprite> Sprites;
    public List<Sprite> FillSprites;
    [HideInInspector]
    public SpriteRenderer Feature, FeatureFill;

    private bool isAvailable;
    public bool IsAvailable
    {
        get { return isAvailable; }
        set
        {
            isAvailable = value;

            if (animator)
                animator.SetBool("Available", value);
            if (colliderComponent)
                colliderComponent.enabled = value;
        }
    }

    protected Animator animator;
    protected BoxCollider colliderComponent;
    #endregion

    void Awake()
    {
        IsAvailable = false;

        GameObject FeatureChild = transform.Find("Feature")?.gameObject;
        Feature = FeatureChild?.GetComponent<SpriteRenderer>();
        FeatureFill = FeatureChild?.transform.GetChild(0)?.gameObject.GetComponent<SpriteRenderer>();

        // 아래에 Feature 리스트 2개를 동일한 방식으로 랜덤하게 섞는 코드도 있으면 좋을 듯
    }

    /// <summary>
    /// Start()는 필드 오브젝트용, OnEnable()은 프리팹용 메서드임. 이렇게 작성해야 잘 동작함.
    /// 따라서, 필드 오브젝트가 하나도 배치되어 있지 않음을 보장할 수 있다면 Start()를 지워도 됨.
    /// </summary>
    

    protected virtual void Start()
    {
        /*
        // 이벤트 주도적 프로그래밍
        MainManager.Instance.AddListener(Event.ObjectAccessed, OnEvent);

        animator = GetComponent<Animator>();
        colliderComponent = GetComponent<BoxCollider>();
        colliderComponent.enabled = false;

        StartCoroutine(AttractNearbyObjects());
        */
    }

    protected virtual void OnEnable()
    {
        // 이벤트 주도적 프로그래밍
        MainManager.Instance.AddListener(Event.ObjectAccessed, OnEvent);

        animator = GetComponent<Animator>();
        colliderComponent = GetComponent<BoxCollider>();
        colliderComponent.enabled = false;

        StartCoroutine(AttractNearbyObjects());
    }

    void Update()
    {
        animator.SetBool("Available", IsAvailable);
    }

    protected virtual void OnDestroy()
    {
        MainManager.Instance.RemoveListener(Event.ObjectAccessed, OnEvent);
    }

    #region Evnet Driven Programming
    public virtual void OnEvent(Event EventType, Component Sender, object Param = null)
    {
        switch (EventType)
        {
            case Event.ObjectAccessed:
                switch (MainManager.State)
                {
                    case Phase.Idle: case Phase.Selected:
                        IsAvailable = false;
                        break;
                    case Phase.Activated:
                        SetObjectAvailability();
                        break;
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
    #endregion

    #region Public Methods
    public virtual void Attract(GameObject targetObject, GameObject accessedObject) // MainManager.cs에서 참조
    {
        Living targetObjectManager = targetObject.GetComponent<Living>();

        targetObjectManager.IsInteracting = true;
        targetObjectManager.InteractingObject = accessedObject;
        targetObjectManager.TargetPosition = new Vector3(
            transform.position.x,
            targetObjectManager.transform.position.y,
            transform.position.z - InteractDistance
        );

        targetObjectManager.IsMovingRight = accessedObject.transform.position.x > targetObject.transform.position.x;
    }

    public virtual void PlayAnimation(Living targetObjectManager) // Living.cs에서 참조
    {
        /* ... */
    }
    #endregion

    #region Animation Event
    public void Vibrate()
    {
        if (MainManager.ActivatedObject == gameObject)
            Vibration.Vibrate((long)2);
    }
    #endregion

    #region Coroutine
    IEnumerator AttractNearbyObjects()
    {
        while (true)
        {
            // 범위 이내의 오브젝트를 감지
            Collider[] colliders = Array.FindAll(
                Physics.OverlapBox(
                    transform.position,
                    new Vector3(InteractBoxSize, InteractBoxSize, InteractBoxSize)
                ),
                collider => collider.gameObject.tag == "Living"
                         && collider.gameObject != MainManager.ActivatedObject
                         && collider.GetComponent<Living>().TargetObjectList.Any(t => t.name.StartsWith(gameObject.name))
            );

            if (colliders.Length > 0)
            {
                GameObject targetObject = colliders[UnityEngine.Random.Range(0, colliders.Length)].gameObject;
                Attract(targetObject, gameObject);
            }

            yield return new WaitForSeconds(ObjectAttractCycle);
        }
    }
    #endregion
}
