using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subway : MonoBehaviour
{
    #region Fields
    public float Speed;
    private int direction;

    private float minValue, maxValue;
    private float timer;
    #endregion

    void Awake()
    {
        timer = 0;
    }

    void Start()
    {
        direction = transform.localScale.x > 0 ? 1 : -1; 

        minValue = MainCamera.GetRenderWidth(transform.GetChild(0).gameObject).Left;
        maxValue = MainCamera.GetRenderWidth(transform.GetChild(transform.childCount - 1).gameObject).Right;

        StartCoroutine(Move());
    }

    void Update()
    {
        if (isOutOfRange(gameObject))
        {
            timer += Time.deltaTime;

            if (timer >= 3)
                Destroy(gameObject);
        }
        else
            timer = 0;
    }

    #region Methods
    bool isOutOfRange(GameObject targetObject)
    {
        bool isOutOfRange = true;

        for (int i=0; i<targetObject.transform.childCount - 1; i++)
        {
            float targetPosX = targetObject.transform.GetChild(i).transform.position.x;
            
            if (minValue < targetPosX && targetPosX < maxValue)
            {
                isOutOfRange = false;
                break;
            }
        }

        return isOutOfRange;
    }
    #endregion

    #region 
    IEnumerator Move()
    {
        while (true)
        {
            transform.Translate(Speed * direction, 0, 0);

            yield return new WaitForSeconds(1/12);
        }
    }
    #endregion
}
