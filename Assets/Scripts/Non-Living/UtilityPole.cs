using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityPole : NonLiving
{
    #region Fields
    [Header("Possible objects")]
    public List<GameObject> Objects;
    public bool IsOpposite;
    #endregion

    protected override void Start()
    {
        if (IsOpposite)
        {
            animator.SetTrigger("GetTransparent");

            MainManager.Instance.AddListener(Event.SettingsEntered, OnEvent);
        }
    }

    #region EvnetDrivenProgramming
    public override void OnEvent(Event EventType, Component Sender, object Param = null)
    {
        switch (EventType)
        {
            case Event.SettingsEntered:
                animator.SetTrigger(MainManager.IsInSettings ? "Appear" : "Disappear");
                break;
        }
    }
    #endregion

    protected override void OnDestroy()
    {
        MainManager.Instance.RemoveListener(Event.SettingsEntered, OnEvent);
    }

    public void SetFeature() // MapGenerator.cs에서 참조
    {
        int order = (int)((transform.position.x + transform.position.z) % Sprites.Count + Sprites.Count)
                  % Sprites.Count;

        Feature.sprite = Sprites[order];
        FeatureFill.sprite = FillSprites[order];
    }
}
