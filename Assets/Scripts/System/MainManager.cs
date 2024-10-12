using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public enum Phase
{
    Initialized, Idle, Selected, Activated
}

public enum Event
{
    SettingsEntered,
    ObjectAccessed
    // 자판기 상호작용, 비둘기 먹이 등등... 추가
}

public class MainManager : MonoBehaviour
{
    #region Fields
    // 싱글톤 응용
    public static MainManager Instance;

    // 이벤트 주도적 프로그래밍
    public delegate void OnEvent(Event eventType, Component Sender, object Param = null);
    private Dictionary<Event, List<OnEvent>> Listeners = new Dictionary<Event, List<OnEvent>>();

    // 공용 변수
    private static bool isInSettings;
    public static bool IsInSettings // MainCamera.cs에서 참조
    {
        get { return isInSettings; }
        set
        {
            isInSettings = value;

            Instance.PostNotification(
                Event.SettingsEntered,
                Instance
            );
            
            Instance.SetUIAppearance(value);
        }
    }

    // 오브젝트 선택
    private static GameObject selectedObject;
    public static GameObject SelectedObject // MainCamera.cs에서 참조
    {
        get { return selectedObject; }
        set
        {
            /*Instance.PostNotification(
                Event.ObjectAccessed,
                selectedObject.GetComponent<Living>()
            );*/

            if (value)
            {
                selectedObject = value;
                selectedObject.GetComponent<Living>().IsSelected = true;
                activatedObject = null;
            }
            else
            {
                if (selectedObject)
                {
                    Living selectedObjectManager = selectedObject.GetComponent<Living>();
                    selectedObjectManager.IsSelected = false;
                }
                selectedObject = value;
            }
        }
    }
    private static GameObject activatedObject;
    public static GameObject ActivatedObject // MainCamera.cs에서 참조
    {
        get { return activatedObject; }
        set
        {
            activatedObject = value;

            if (value)
            {
                ActivatedObjectManager = activatedObject.GetComponent<Living>();
                ActivatedObjectManager.IsActivated = value != null;
                selectedObject = null;
            }
            else
                AccessibleObjectList = new List<GameObject>();
        }
    }
    public static Living ActivatedObjectManager;

    public static GameObject ObjectsParent;

    [HideInInspector]
    // 활성화된 Living 오브젝트가 상호작용 가능한 오브젝트
    public static List<GameObject> AccessibleObjectList;
    public static Phase State;
    private Vector3 mouseDownPos;
    [HideInInspector]
    public MainCamera MainCamera; // Living.cs에서 참조

    [Header("Framerate")]
    public int TargetFrameRate;

    [Header("UI")]
    public List<GameObject> UIList;

    public static bool IsVibrationOn;

    // 디버그
    [Header("Debug")]
    public bool IsDebugging;
    public static TextMeshProUGUI DebugTMP;
    float deltaTime = 0.0f;
    #endregion



    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = TargetFrameRate;
        
        State = Phase.Initialized;
        AccessibleObjectList = new List<GameObject>();

        IsVibrationOn = PlayerPrefs.GetInt("Vibration") == 1 ? true : false;
    }

    void Start()
    {
        MainCamera = GameObject.Find("MainCamera").GetComponent<MainCamera>();
        DebugTMP = GameObject.Find("Debug TMP")?.GetComponentInParent<TextMeshProUGUI>();

        ObjectsParent = GameObject.Find("Objects");
    }
    
    void Update()
    {
        SelectObject();

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    #region Event Driven Programming
    public void AddListener(Event eventType, OnEvent Listener)
    {
        List<OnEvent> ListenList = null;

        if (Listeners.TryGetValue(eventType, out ListenList))
        {
            ListenList.Add(Listener);
            return;
        }

        ListenList = new List<OnEvent>();
        ListenList.Add(Listener);
        Listeners.Add(eventType, ListenList);
    }

    public void PostNotification(Event eventType, Component Sender, object param = null)
    {
        RemoveRedundancies();

        List<OnEvent> ListenList = null;

        if (!Listeners.TryGetValue(eventType, out ListenList))
            return;

        for(int i=0; i<ListenList.Count; i++)
            ListenList?[i](eventType, Sender, param);
    }

    public void RemoveEvent(Event eventType) => Listeners.Remove(eventType);

    public void RemoveRedundancies()
    {
        Dictionary<Event, List<OnEvent>> newListeners = new Dictionary<Event, List<OnEvent>>();

        foreach(KeyValuePair<Event, List<OnEvent>> Item in Listeners)
        {
            for (int i=Item.Value.Count - 1; i>=0; i--)
                if(Item.Value[i].Equals(null))
                    Item.Value.RemoveAt(i);

            if(Item.Value.Count > 0)
                newListeners.Add(Item.Key, Item.Value);
        }

        Listeners = newListeners;
    }

    public void RemoveListener(Event eventType, OnEvent listener)
    {
        List<OnEvent> listenList = null;
        if (Listeners.TryGetValue(eventType, out listenList))
        {
            listenList.Remove(listener);
        }
    }

    void OnLevelWasLoaded()
    {
        RemoveRedundancies();
    }
    #endregion

    #region ObjectSelection
    void SelectObject()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
#else
        if (Input.touchCount > 0)
#endif
        {
#if UNITY_EDITOR
            mouseDownPos = Input.mousePosition;
#else
            mouseDownPos = Input.GetTouch(0).position;
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif
            Ray ray = Camera.main.ScreenPointToRay(mouseDownPos);
            RaycastHit hit;
 
            if (Physics.Raycast(ray, out hit))
            {
                if (MainManager.IsVibrationOn)
                    Vibration.Vibrate((long)5);
                
                GameObject accessedObject = hit.collider.gameObject;

                if (accessedObject.tag == "Living")
                {
                    Living hitObjectManager = accessedObject.GetComponent<Living>();

                    switch (State)
                    {
                        case Phase.Activated:
                            if (accessedObject != ActivatedObject)
                            {
                                State = Phase.Selected;
                                ActivatedObjectManager.IsInteracting = false;
                                AccessibleObjectList = new List<GameObject>();
                            }
                            break;
                        case Phase.Selected:
                            if (accessedObject == SelectedObject)
                            {
                                State = Phase.Activated;
                                AccessibleObjectList = hitObjectManager.TargetObjectList;
                            }
                            break;
                        case Phase.Idle:
                            State = Phase.Selected;
                            break;
                    }

                    if (!IsInSettings)
                    {
                        Instance.PostNotification(
                            Event.ObjectAccessed,
                            hitObjectManager
                        );
                    }
                }
                else if (accessedObject.tag == "NonLiving")
                    accessedObject.GetComponent<NonLiving>().Attract(ActivatedObject, accessedObject);
            }
            else
            {
                if (MainManager.State == Phase.Selected)
                {
                    State = Phase.Idle;
                    SelectedObject = null;
                }
            }
        }
    }
    #endregion

    #region Methods
    void SetUIAppearance(bool isInSettings)
    {
        foreach (GameObject UIObject in UIList)
            UIObject.GetComponent<Animator>().SetBool("Appear", isInSettings);
    }
    #endregion

    #region Public Methods
    public Vector3 GetImageSize(GameObject targetObject)
    {
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = targetObject.GetComponentInChildren<SpriteRenderer>();

        Vector2 spriteSize = spriteRenderer.sprite.rect.size;

        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
        Vector2 worldSize = new Vector2(spriteSize.x / pixelsPerUnit, spriteSize.y / pixelsPerUnit);

        Vector3 finalSize = new Vector3(
            worldSize.x * spriteRenderer.transform.localScale.x,
            worldSize.y * spriteRenderer.transform.localScale.y,
            1
        );

        return finalSize;
    }

    // 디버그용
    public void DebugButtonOnClick()
    {
        IsInSettings = !IsInSettings;
    }

    /*
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
 
        GUIStyle style = new GUIStyle();
 
        Rect rect = new Rect(100, 100, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
    */
    #endregion
}



/* 메모장

#region Debug
[MenuItem("CustomMenu/Debug")]
static void CheckCurrentState()
{
    Debug.Log(MainManager.Instance.State);
}
#endregion

*/