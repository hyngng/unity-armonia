using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    #region Fields
    public static Dictionary<string, int> Population;

    [Header("Standard")]
    public GameObject standardObject;

    [Header("Object generatation")]
    public List<GameObject> Livings;
    public List<GameObject> NonLivings;
    [Range(0, 10)]
    public float GenerationDelay;
    public static List<GameObject> GeneratedLivings, GeneratedNonLivings; // MainManager.cs에서 참조

    [HideInInspector]
    public int NumberOfLivings, NumberOfNonLivings;

    [Header("Population Limit")]
    [Range(0, 100)]
    public List<int> MaxPopulationCount;
    #endregion

    void Awake()
    {
        NumberOfLivings    = 0;
        NumberOfNonLivings = 0;

        GeneratedLivings = new List<GameObject>();
        GeneratedNonLivings = new List<GameObject>();

        Population = new Dictionary<string, int>();
        for (int i=0; i<Livings.Count; i++)
            Population.Add(Livings[i].GetComponent<Living>().GetType().Name, 0);
    }

    void Start()
    {
        StartCoroutine(ManagePopulation());
    }

    IEnumerator ManagePopulation()
    {
        while (true)
        {
            foreach (GameObject living in Livings)
            {
                int index = Livings.IndexOf(living);

                if (Population[living.GetComponent<Living>().GetType().Name] < MaxPopulationCount[index])
                    GenerateLiving(Livings[index]);
            }

            yield return new WaitForSeconds(GenerationDelay);
        }
    }

    void GenerateLiving(GameObject targetObject)
    {
        bool spawnAtLeft = Random.value > .5f;
        float spawnPosX = spawnAtLeft
                        ? MainCamera.GetRenderWidth(gameObject).Left - 1.8f
                        : MainCamera.GetRenderWidth(gameObject).Right + 1.8f;

        GameObject generatedLivingObject = Instantiate(
            targetObject,
            new Vector3(spawnPosX, targetObject.GetComponent<BoxCollider>().size.y / 2, Random.Range(-3.5f, 3.5f)),
            Quaternion.identity
        );
        generatedLivingObject.transform.parent = standardObject.transform;
        GeneratedLivings.Add(generatedLivingObject);

        if (spawnAtLeft)
            generatedLivingObject.GetComponent<Living>().IsMovingRight = true;
    }

    void GenerateNonLiving()
    {
        // NonLiving 생성하는 코드 작성
    }
}
