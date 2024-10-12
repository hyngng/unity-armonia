using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : Living
{
    #region Fields
    [Header("Features: Hat")]
    [Range(0, 1)]
    public float HatRatio;
    public GameObject Head;
    public List<Sprite> Hats;

    [Header("Features: Cloth")]
    public GameObject Body;
    public List<Sprite> Clothes;

    [Header("Features: Shoes")]
    [Range(0, 1)]
    public float ShoesRatio;
    public GameObject LeftFoot;
    public List<Sprite> LeftShoes;
    public GameObject RightFoot;
    public List<Sprite> RightShoes;

    [Header("Features: Bag")]
    [Range(0, 1)]
    public float BagRatio;
    public GameObject Bag;
    public List<Sprite> Bags;
    public List<Sprite> BagFills;

    private int index;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new PeopleStateMachine(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 상태 패턴
        StateMachine.Initialize(StateMachine.IdleState);

        // 모자 쓰기
        bool hasHat = Hats.Count > 0;
        if (hasHat)
        {
            index = Random.Range(0, Hats.Count);

            if (Random.value < HatRatio)
                Head.transform.Find("Hat").GetComponent<SpriteRenderer>().sprite = Hats[index];
        }

        // 신발 신기
        bool hasShoes = LeftShoes.Count > 0 && RightShoes.Count > 0;
        if (hasShoes)
        {
            index = Random.Range(0, LeftShoes.Count);

            if (Random.value < ShoesRatio)
            {
                LeftFoot.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = LeftShoes[index];
                RightFoot.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = RightShoes[index];
            }
        }

        // 가방 메기
        bool hasBag = Bags.Count > 0;
        if (hasBag)
        {
            index = Random.Range(0, Bags.Count);

            if (Random.value < BagRatio)
            {
                Bag.GetComponent<SpriteRenderer>().sprite = Bags[index];
                Bag.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = BagFills[index];
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        // 상태 패턴 때문에 Living.cs가 아닌 여기에 작성해야 함. 고쳐야 할 것 같은데...아님 계속 반복될 것 같은데...
        if (IsInteracting)
            if (Mathf.Approximately(transform.position.x, TargetPosition.x))
                if (StateMachine.CurrentState is IdleState or WalkState)
                    StateMachine.TransitionTo(((PeopleStateMachine)StateMachine).PeopleVendingMachineState);
                    // 형변환 해서 사용해야 함
    }

    public override void StopInteraction()
    {
        base.StopInteraction();

        StateMachine.TransitionTo(StateMachine.IdleState);
    }
}
