using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    #region Fields
    [Header("System")]
    public float ZoomIntensity;

    public static float DragDistance;
    public static float PinchDelta; // MainCamFover.cs에서 참조함
    private float pinchDistance; // 아직 아무도 참조안함
    private Touch firstTouch, secondTouch;
    #endregion

    void Awake()
    {
        firstTouch  = default;
        secondTouch = default;
    }

    void Update()
    {
        switch (Input.touchCount)
        {
            default: case 0:
                // 터치 입력이 없을 때
                PinchDelta = 0;
                pinchDistance = 0;
                DragDistance = 0;

                firstTouch  = default;
                secondTouch = default;
                break;
            case 1:
                // 한 손가락 터치입력이 들어왔을 때
                PinchDelta = 0;
                pinchDistance = 0;

                firstTouch  = Input.touches[0];
                secondTouch = default;

                DragDistance = (firstTouch.position - firstTouch.deltaPosition).magnitude;

                break;
            case 2:
                // 두 손가락 터치입력이 들어왔을 때
                DragDistance = 0;
                
                firstTouch  = Input.touches[0];
                secondTouch = Input.touches[1];

                Vector2 t1PrevPos = firstTouch.position - firstTouch.deltaPosition;
                Vector2 t2PrevPos = secondTouch.position - secondTouch.deltaPosition;

                float prevDeltaMag = (t1PrevPos - t2PrevPos).magnitude;
                float deltaMag = (firstTouch.position - secondTouch.position).magnitude;

                PinchDelta = (prevDeltaMag - deltaMag) * ZoomIntensity;
                pinchDistance += PinchDelta;
                
                HandleSettingsEntry();
                break;
        }
    }

    #region Methods
    void HandleSettingsEntry()
    {
        if (MainManager.IsInSettings && pinchDistance <= -10)
        {
            if (MainManager.IsVibrationOn)
                Vibration.Vibrate((long)10);

            MainManager.IsInSettings = false;
            pinchDistance = 0;
        }
        else if (!MainManager.IsInSettings && pinchDistance >= 20)
        {
            if (MainManager.IsVibrationOn)
                Vibration.Vibrate((long)10);
                
            MainManager.IsInSettings = true;
            pinchDistance = 0;
        }
    }
    #endregion
}
