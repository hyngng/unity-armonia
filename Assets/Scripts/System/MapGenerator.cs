using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Fields
    [Header("Standard")]
    public GameObject StandardObject;

    [Header("Roads")]
    [Tooltip("첫 번째 오브젝트는 시작 오브젝트가 됨")]
    public List<GameObject> RoadObjects;
    private List<GameObject> roads;
    private float roadSize;

    [Header("Buildings")]
    public List<GameObject> BuildingObjects;
    public List<GameObject> BackgroundBuildingObjects;
    private List<GameObject> buildings;

    private List<GameObject> unitObjects, units;
    private float unitWidth;
    private float unitLayerWidth;
    [SerializeField]
    [Tooltip("빌딩의 앞뒤간격을 조절하는 변수")]
    public static float LayerGap; // Unit.cs에서 참조

    [Header("Utility Poles")]
    public GameObject UtilityPoleObject;
    private List<GameObject> utilityPoles;

    public static List<int> RandomNumberList;
    private new Camera camera;
    #endregion

    void Awake()
    {
        roads = new List<GameObject>();
        units = new List<GameObject>();

        unitObjects = new List<GameObject>();
        unitObjects.Add(new GameObject() { name = "Unit" });
    }

    void Start()
    {
        camera = GameObject.Find("MainCamera").GetComponent<Camera>();

        // 랜덤 숫자 리스트 생성
        RandomNumberList = Enumerable.Range(0, 11).OrderBy(x => Random.value).ToList();

        // bigUnit 너비값 구하기
        float marginByUtilityPole = MainManager.Instance.GetImageSize(UtilityPoleObject).x / 2;
        float allBuildingsWidth = BuildingObjects.Sum(obj => MainManager.Instance.GetImageSize(obj).x);
        unitWidth = marginByUtilityPole + allBuildingsWidth + (BuildingObjects.Count - 1) * marginByUtilityPole;

        float allBackgroundBuildingsWidth = BackgroundBuildingObjects.Sum(obj => MainManager.Instance.GetImageSize(obj).x);
        unitLayerWidth = allBackgroundBuildingsWidth;

        // road 너비값 구하기
        roadSize = MainManager.Instance.GetImageSize(RoadObjects[0]).x;

        GenerateObjects(roads, RoadObjects, roadSize);
        GenerateObjects(units, unitObjects, unitWidth);
    }

    void Update()
    {
        GenerateObjects(roads, RoadObjects, roadSize);
        GenerateObjects(units, unitObjects, unitWidth);
    }

    #region Methods
    void GenerateObjects(List<GameObject> instantiated, List<GameObject> instantiable, float objectSize)
    {
        if (instantiated.Count == 0)
        {
            // 기준점 오브젝트 생성
            GameObject tempObject = Instantiate(instantiable[0], new Vector3(0, 0, 0), Quaternion.identity);
            tempObject.transform.parent = StandardObject.transform;
            instantiated.Add(tempObject);

            SetUnitData(tempObject);

            return;
        }

        bool ObjectsNotEnough = MainCamera.GetRenderWidth(gameObject).Right - MainCamera.GetRenderWidth(gameObject).Left
                              > instantiated[instantiated.Count - 1].transform.position.x - instantiated[0].transform.position.x;

        if (instantiated[0].transform.position.x - objectSize / 2 > MainCamera.GetRenderWidth(gameObject).Left)
        {
            // 화면 좌측
            if (ObjectsNotEnough)
            {
                // 주어진 화각 내에서 오브젝트가 부족할 때
                instantiated.Add(null);

                for (int i=instantiated.Count - 1; i>0; i--)
                    instantiated[i] = instantiated[i - 1];

                instantiated[0] = Instantiate(
                    instantiable[0],
                    new Vector3(
                        instantiated[0].transform.position.x - objectSize, 0, 0
                    ),
                    Quaternion.identity
                );
                instantiated[0].transform.parent = StandardObject.transform;
            }
            else if (instantiated.Count >= 2)
            {
                GameObject tempinstantiated = instantiated[instantiated.Count - 1];

                for (int i=instantiated.Count - 1; i>0; i--)
                    instantiated[i] = instantiated[i - 1];
                instantiated[0] = tempinstantiated;
                
                instantiated[0].transform.position = new Vector3(instantiated[1].transform.position.x - objectSize, 0, 0);
            }

            if (instantiated[0])
                SetUnitData(instantiated[0]);
            // SetPoleAppearance(0);
        }
        else if (instantiated[instantiated.Count - 1].transform.position.x + objectSize / 2 < MainCamera.GetRenderWidth(gameObject).Right)
        {
            // 화면 우측
            if (ObjectsNotEnough)
            {
                // 주어진 화각 내에서 오브젝트가 부족할 때
                instantiated.Add(Instantiate(instantiable[0], new Vector3(
                    instantiated[instantiated.Count - 1].transform.position.x + objectSize, 0, 0), Quaternion.identity)
                );
                instantiated[instantiated.Count - 1].transform.parent = StandardObject.transform;
            }
            else if (instantiated.Count >= 2)
            {
                GameObject tempinstantiated = instantiated[0];

                for (int i=0; i<instantiated.Count - 1; i++)
                    instantiated[i] = instantiated[i + 1];

                instantiated[instantiated.Count - 1] = tempinstantiated;
                instantiated[instantiated.Count - 1].transform.position = new Vector3(
                    instantiated[instantiated.Count - 2].transform.position.x + objectSize, 0, 0
                );
            }

            if (instantiated[instantiated.Count - 1])
                SetUnitData(instantiated[instantiated.Count - 1]);
            // SetPoleAppearance(roads.Count - 1);
        }
    }
    
    void SetUnitData(GameObject targetObject)
    {
        if (targetObject.name.Contains("Unit"))
        {
            Unit unitManager = targetObject.GetComponent<Unit>();
            if (!unitManager)
                unitManager = targetObject.AddComponent<Unit>();

            unitManager.Width = unitWidth;
            unitManager.LayerWidth = unitLayerWidth;
            unitManager.Space = MainManager.Instance.GetImageSize(UtilityPoleObject).x * .5f;
            unitManager.GeneratableBuildings = BuildingObjects;
            unitManager.GeneratableBackgroundBuildings = BackgroundBuildingObjects;
            unitManager.UtilityPole = UtilityPoleObject;
        }
    }
    #endregion
}