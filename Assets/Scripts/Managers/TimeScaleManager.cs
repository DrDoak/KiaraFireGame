using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    public static TimeScaleManager Instance;

    public static float TimeScale { get { return Instance.isFrozen? 0 : Instance.currentTimeScale; } }
    public bool IsFrozen { get { return isFrozen; } }
    private float currentTimeScale = 1f;
    private float freezeExpiration;
    private bool isFrozen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public static void SetTimeScale(float newTimeScale)
    {
        Instance.currentTimeScale = newTimeScale;
    }
    public static void FreezeTime(float duration)
    {
        Instance.freezeExpiration = Time.timeSinceLevelLoad + duration;
        Instance.isFrozen = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isFrozen)
        {
            if (Time.timeSinceLevelLoad > Instance.freezeExpiration)
            {
                isFrozen = false;
            }
        }
    }
}
