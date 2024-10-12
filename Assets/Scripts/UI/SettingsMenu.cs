using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    #region Fields
    [Header("Time")]
    public GameObject TimerObject;
    private TextMeshProUGUI timerTMPro;
    [Tooltip("단위는 초.")]
    public int TimeCheckTime;

    [Header("Battery")]
    public GameObject BatteryLevelObject;
    private Image batteryLevelImage;
    public int BatteryCheckTime;
    public Sprite[] BatteryLevelImages;
    [Tooltip("단위는 초. 정식 빌드 시에는 60초 정도로 조정할 것")]

    [Header("ShutterSpeed")]
    public GameObject ShutterSpeedObject;
    private Animator shutterSpeedAnimator;
    private TextMeshProUGUI shutterSpeedTMPro;
    private Dictionary<int, string> shutterSpeedDictionary;
    public string[] ShutterSpeedArray;
    private int shutterSpeedIndex;

    [Header("Aperture")]
    public GameObject ApertureObject;
    private Animator apertureAnimator;
    private TextMeshProUGUI apertureTMPro;
    private Dictionary<int, string> apertureDictionary;
    public string[] ApertureArray;
    private int apertureIndex;

    [Header("Vibration")]
    public GameObject VibrationObject;
    private Animator vibrationAnimator;
    private Image vibrationImage;
    #endregion

    void Start()
    {
        batteryLevelImage = BatteryLevelObject.GetComponent<Image>();
        timerTMPro = TimerObject.GetComponent<TextMeshProUGUI>();
        shutterSpeedTMPro = ShutterSpeedObject.GetComponent<TextMeshProUGUI>();
        apertureTMPro = ApertureObject.GetComponent<TextMeshProUGUI>();
        vibrationImage = VibrationObject.GetComponent<Image>();

        shutterSpeedAnimator = ShutterSpeedObject.transform.parent.gameObject.GetComponent<Animator>();
        apertureAnimator = ApertureObject.transform.parent.gameObject.GetComponent<Animator>();
        vibrationAnimator = VibrationObject.transform.parent.gameObject.GetComponent<Animator>();

        shutterSpeedDictionary = new Dictionary<int, string>();
        for (int i=0; i<ShutterSpeedArray.Length; i++)
            shutterSpeedDictionary.Add(i, ShutterSpeedArray[i]);

        apertureDictionary = new Dictionary<int, string>();
        for (int i=0; i<ApertureArray.Length; i++)
            apertureDictionary.Add(i, ApertureArray[i]);
        
        // 초기화
        shutterSpeedIndex = PlayerPrefs.GetInt("ShutterSpeed");
        apertureIndex = PlayerPrefs.GetInt("Aperture");
        SetMotionBlur(shutterSpeedIndex);

        shutterSpeedTMPro.text = shutterSpeedDictionary[shutterSpeedIndex];
        apertureTMPro.text = apertureDictionary[apertureIndex];
        SetAperture(apertureIndex);

        Color newColor = vibrationImage.color;
        newColor.a = PlayerPrefs.GetInt("Vibration") == 1 ? 1 : 0;
        vibrationImage.color = newColor;

        StartCoroutine(TimeCheck());
        StartCoroutine(BatteryCheck());
    }

    #region Methods
    void SetMotionBlur(int input)
    {
        //
    }

    void SetAperture(int input)
    {
        //
    }
    #endregion

    #region Public Methods
    // 버튼 이벤트
    public void ShutterSpeedOnClick()
    {
        if (MainManager.IsVibrationOn)
            Vibration.Vibrate((long)10);
        shutterSpeedAnimator.SetTrigger("Clicked");

        shutterSpeedIndex += 1;
        if (shutterSpeedIndex == shutterSpeedDictionary.Count)
            shutterSpeedIndex = 0;
        
        PlayerPrefs.SetInt("ShutterSpeed", shutterSpeedIndex);

        shutterSpeedTMPro.text = shutterSpeedDictionary[shutterSpeedIndex];

        SetMotionBlur(shutterSpeedIndex);
    }

    public void ApertureOnClick()
    {
        if (MainManager.IsVibrationOn)
            Vibration.Vibrate((long)10);
        apertureAnimator.SetTrigger("Clicked");

        apertureIndex += 1;
        if (apertureIndex == apertureDictionary.Count)
            apertureIndex = 0;
        
        PlayerPrefs.SetInt("Aperture", apertureIndex);

        apertureTMPro.text = apertureDictionary[apertureIndex];

        SetAperture(apertureIndex);
    }

    public void VibrationOnClick()
    {
        Vibration.Vibrate((long)10);
        vibrationAnimator.SetTrigger("Clicked");

        MainManager.IsVibrationOn = !MainManager.IsVibrationOn;
        PlayerPrefs.GetInt("Vibration", MainManager.IsVibrationOn ? 1 : 0);

        Color newColor = vibrationImage.color;
        newColor.a = MainManager.IsVibrationOn ? 1 : 0;
        vibrationImage.color = newColor;
    }
    #endregion

    IEnumerator TimeCheck()
    {
        while (true)
        {
            timerTMPro.text = DateTime.Now.ToString(("HH:mm"));
            yield return new WaitForSeconds(TimeCheckTime);
        }
    }

    IEnumerator BatteryCheck()
    {
        while (true)
        {
            float batteryLevel = SystemInfo.batteryLevel; // 베터리 충전량 가져오기 (0 ~ 1);
        
            if (batteryLevel > .75f)
                batteryLevelImage.sprite = BatteryLevelImages[4];
            else if (batteryLevel > .5f)
                batteryLevelImage.sprite = BatteryLevelImages[3];
            else if (batteryLevel > .25f)
                batteryLevelImage.sprite = BatteryLevelImages[2];  
            else if (batteryLevel > .1f)
                batteryLevelImage.sprite = BatteryLevelImages[1];
            else
                batteryLevelImage.sprite = BatteryLevelImages[0];

            yield return new WaitForSeconds(BatteryCheckTime);
        }
    }
}
