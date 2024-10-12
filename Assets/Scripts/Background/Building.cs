using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    #region Field
    [Header("Nearby Objects")]
    public GameObject VentilatorObject;
    public GameObject[] GroundObjects;

    private Transform ventilatorsPos, groundObjectsPos;
    #endregion

    void OnEnable()
    {
        ventilatorsPos = transform.Find("Ventilators Pos");
        groundObjectsPos = transform.Find("Ground Objects Pos");

        SetVentilatorObjects();
        SetGroundObjects();
    }

    #region Public Methods
    public void SetVentilatorObjects()
    {
        // 환풍기
        if (ventilatorsPos)
        {
            foreach (Transform child in ventilatorsPos)
                if (child.childCount > 0)
                    Destroy(child.GetChild(0).gameObject);

            Transform[] ventilatorsPosArray = ventilatorsPos.GetComponentsInChildren<Transform>();
            int number = (int)Mathf.Abs(transform.position.x);

            for (int i=0; i<ventilatorsPosArray.Length; i++)
            {
                if (number % 10 > 5)
                {
                    GameObject instantiatedObject = Instantiate(VentilatorObject);
                    instantiatedObject.transform.parent = ventilatorsPosArray[i];
                    instantiatedObject.transform.localPosition = Vector3.zero;
                }

                number += 7;
            }
        }
    }
    public void SetGroundObjects()
    {
        // 땅 오브젝트
        if (groundObjectsPos && GroundObjects.Length > 0)
        {
            foreach (Transform child in groundObjectsPos)
                if (child.childCount > 0)
                    Destroy(child.GetChild(0).gameObject);

            Transform[] groundObjectsPosArray = groundObjectsPos.GetComponentsInChildren<Transform>();
            int number = (int)Mathf.Abs(transform.position.x);

            for (int i=0; i<groundObjectsPosArray.Length; i++)
            {
                if (number % 10 > 5)
                {
                    GameObject instantiatedObject = Instantiate(GroundObjects[number % GroundObjects.Length]);
                    instantiatedObject.transform.parent = groundObjectsPosArray[i];
                    instantiatedObject.transform.localPosition = Vector3.zero;
                }

                number += 7;
            }
        }
    }
    #endregion
}
