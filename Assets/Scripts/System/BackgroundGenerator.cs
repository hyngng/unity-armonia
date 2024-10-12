using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    #region Fields
    [Header("Standard")]
    public GameObject standardObject;

    [Header("Subway")]
    private GameObject[] subways;
    public GameObject SubwayCar;
    private Vector3 subwayCarSize;
    public GameObject Bridge;
    private Vector3 bridgeSize;
    public float Speed;
    [Range(1, 10)]
    public int SubwayCarCount;
    [Tooltip("지하철 스폰 간격 범위, 단위는 초(second)")]
    public float MinSpawnInterval, MaxSpawnInterval;
    public float RailwayGap;

    private List<GameObject> bridges;
    #endregion

    void Start()
    {
        subways = new GameObject[2];
        bridges = new List<GameObject>();

        // 이미지 크기 구하기
        subwayCarSize = MainManager.Instance.GetImageSize(SubwayCar);
        bridgeSize = MainManager.Instance.GetImageSize(Bridge);

        // 기준점이 되는 Bridge 오브젝트 생성
        GameObject standardBridge = Instantiate(Bridge, Bridge.transform.position, Quaternion.Euler(0, 0, 0));
        standardBridge.transform.parent = standardObject.transform;
        bridges.Add(standardBridge);

        // 코루틴
        StartCoroutine(GenerateSubway());
        
        // 함수
        GenerateBridges();
    }

    void Update()
    {
        GenerateBridges();
    }

    void GenerateBridges()
    {
        bool ObjectsNotEnough = bridges[bridges.Count - 1].transform.position.x - bridges[0].transform.position.x
                              < MainCamera.GetRenderWidth(Bridge).Right - MainCamera.GetRenderWidth(Bridge).Left;

        if (MainCamera.GetRenderWidth(Bridge).Left < bridges[0].transform.position.x - bridgeSize.x / 2)
        {
            if (ObjectsNotEnough)
            {
                // 주어진 화각 내에서 오브젝트가 부족할 때
                bridges.Add(null);

                for (int i=bridges.Count - 1; i>0; i--)
                    bridges[i] = bridges[i - 1];

                bridges[0] = Instantiate(
                    Bridge,
                    new Vector3(
                        bridges[0].transform.position.x - bridgeSize.x,
                        Bridge.transform.position.y,
                        Bridge.transform.position.z
                    ),
                    Quaternion.Euler(0, 0, 0)
                );
                bridges[0].transform.parent = standardObject.transform;
            }
            else if (bridges.Count > 2)
            {
                GameObject tempBridge = bridges[bridges.Count - 1];

                for (int i=bridges.Count - 1; i>0; i--)
                    bridges[i] = bridges[i - 1];
                bridges[0] = tempBridge;
                
                bridges[0].transform.position = new Vector3(
                    bridges[1].transform.position.x - bridgeSize.x,
                    bridges[0].transform.position.y,
                    bridges[0].transform.position.z
                );
            }
        }
        else if (bridges[bridges.Count - 1].transform.position.x + bridgeSize.x / 2 < MainCamera.GetRenderWidth(Bridge).Right)
        {
            if (ObjectsNotEnough)
            {
                // 주어진 화각 내에서 오브젝트가 부족할 때
                bridges.Add(Instantiate(
                    Bridge,
                    new Vector3(
                        bridges[bridges.Count - 1].transform.position.x + bridgeSize.x,
                        Bridge.transform.position.y,
                        Bridge.transform.position.z
                    ),
                    Quaternion.Euler(0, 0, 0)
                ));
                bridges[bridges.Count - 1].transform.parent = standardObject.transform;
            }
            else if (bridges.Count > 2)
            {
                GameObject tempBridge = bridges[0];

                for (int i=0; i<bridges.Count - 1; i++)
                    bridges[i] = bridges[i + 1];

                bridges[bridges.Count - 1] = tempBridge;
                bridges[bridges.Count - 1].transform.position = new Vector3(
                    bridges[bridges.Count - 2].transform.position.x + bridgeSize.x,
                    bridges[bridges.Count - 1].transform.position.y,
                    bridges[bridges.Count - 1].transform.position.z
                );
            }
        }
    }

    #region Coroutine
    IEnumerator GenerateSubway()
    {
        while (true)
        {
            bool subwaysArrayHasEmpty = (subways[0] == null || subways[1] == null);

            if (subwaysArrayHasEmpty)
            {
                // 기준점 지하철 패키지 오브젝트 스폰 위치
                bool spawnLeft = subways[0] == null ? true : false;

                Vector3 spawnPosition = (spawnLeft
                    ? new Vector3(
                        bridges[0].transform.position.x - subwayCarSize.x,
                        bridges[0].transform.position.y,
                        bridges[0].transform.position.z
                    )
                    : new Vector3(
                        bridges[bridges.Count - 1].transform.position.x + subwayCarSize.x,
                        bridges[bridges.Count - 1].transform.position.y,
                        bridges[bridges.Count - 1].transform.position.z
                    )
                );
                spawnPosition = new Vector3(
                    spawnPosition.x,
                    SubwayCar.transform.position.y,
                    spawnLeft ? SubwayCar.transform.position.z : SubwayCar.transform.position.z + RailwayGap
                );

                // 기준점 지하철 패키지 오브젝트 생성
                GameObject subway = new GameObject();
                subway.transform.position = spawnPosition;
                subway.transform.parent = standardObject.transform;
                subway.name = "Subway";

                // 지하철 칸 생성
                for (int i=0; i<SubwayCarCount; i++)
                {
                    GameObject generatedSubwayCar = Instantiate(SubwayCar);
                    generatedSubwayCar.transform.parent = subway.transform;
                    generatedSubwayCar.transform.localPosition = new Vector3(SubwayCar.transform.position.x - i * subwayCarSize.x, 0, 0);
                }

                // 지하철 방향 설정
                subway.transform.localScale = new Vector3(
                    spawnLeft ? subway.transform.localScale.x : subway.transform.localScale.x * -1,
                    subway.transform.localScale.y,
                    subway.transform.localScale.z
                );

                // 스크립트 부착
                Subway subwayManager = subway.AddComponent<Subway>();
                subwayManager.Speed = this.Speed;
                
                // 지하철 등록
                subways[spawnLeft ? 0 : 1] = subway;
            }

            yield return new WaitForSeconds(Random.Range(MinSpawnInterval, MaxSpawnInterval));
        }
    }
    #endregion
}
