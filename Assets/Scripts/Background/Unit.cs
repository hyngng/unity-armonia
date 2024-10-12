using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region 
    public float Width;
    public float LayerWidth;
    public float Space;

    public List<GameObject> GeneratableBuildings;
    public List<GameObject> GeneratableBackgroundBuildings;
    public GameObject UtilityPole;

    private Vector3 objectPosition;
    public Vector3 ObjectPosition
    {
        get { return objectPosition; }
        set
        {
            if (objectPosition != value)
            {
                objectPosition = value;

                Generate(GeneratableBuildings);
            }
        }
    }
    #endregion

    void Start()
    {
        Generate(GeneratableBuildings);
    }

    void Update()
    {
        // 위치가 바뀔 때마다 Generate() 함수가 실행됨
        ObjectPosition = transform.position;
    }

    #region Public Methods
    public void Generate(List<GameObject> Buildings)
    {
        // 전부 지우고
        for (int i=transform.childCount-1; i>= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
        MainManager.Instance.RemoveRedundancies();

        // 빌딩 생성
        float startPos = transform.position.x - Width / 2;
        float totalLength = startPos + Space;
        
        for (int i=0; i<GeneratableBuildings.Count; i++)
        {
            GameObject buildingToGenerate = GeneratableBuildings[i];
            float targetPos = MainManager.Instance.GetImageSize(buildingToGenerate).x / 2;

            // 건물
            GameObject generatedBuilding = Instantiate(buildingToGenerate);
            generatedBuilding.transform.localPosition = new Vector3(
                totalLength + targetPos,
                buildingToGenerate.transform.position.y,
                buildingToGenerate.transform.position.z
            );
            generatedBuilding.GetComponent<Building>().SetGroundObjects();
            generatedBuilding.transform.parent = transform;

            totalLength += targetPos * 2;

            // 가로등
            GameObject generatedUtilityPole = Instantiate(UtilityPole);
            generatedUtilityPole.transform.localPosition = new Vector3(
                totalLength + Space / 2,
                generatedUtilityPole.transform.position.y,
                generatedUtilityPole.transform.position.z
            );
            generatedUtilityPole.transform.parent = transform;
            generatedUtilityPole.GetComponent<UtilityPole>().SetFeature();

            // 맞은편 가로등
            generatedUtilityPole = Instantiate(UtilityPole);
            generatedUtilityPole.transform.localPosition = new Vector3(
                totalLength + Space / 2,
                generatedUtilityPole.transform.position.y,
                generatedUtilityPole.transform.position.z - (10 + generatedUtilityPole.transform.position.z)
            );
            generatedUtilityPole.transform.parent = transform;
            generatedUtilityPole.GetComponent<UtilityPole>().IsOpposite = true;
            generatedUtilityPole.GetComponent<UtilityPole>().SetFeature();

            totalLength += Space;
        }

        // 뒤에 있는 빌딩 생성
        int loopCount = (int)(Width / LayerWidth);
        float backgroundBuildingGap = (Width - LayerWidth * loopCount) / (GeneratableBackgroundBuildings.Count * loopCount);

        totalLength = startPos + backgroundBuildingGap / 2;

        for (int i=0; i<loopCount; i++)
        {
            for (int j=0; j<GeneratableBackgroundBuildings.Count; j++)
            {
                GameObject buildingToGenerate = GeneratableBackgroundBuildings[j];
                float targetPos = MainManager.Instance.GetImageSize(buildingToGenerate).x / 2;

                // 건물
                GameObject generatedBuilding = Instantiate(buildingToGenerate);
                generatedBuilding.transform.localPosition = new Vector3(
                    totalLength + targetPos,
                    buildingToGenerate.transform.position.y + MapGenerator.LayerGap,
                    buildingToGenerate.transform.position.z
                );
                generatedBuilding.transform.parent = transform;

                totalLength += targetPos * 2;

                // 간격
                totalLength += backgroundBuildingGap / 2;
            }
        }
    }
    #endregion
}