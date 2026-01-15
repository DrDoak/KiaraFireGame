using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableTime : MonoBehaviour
{
    public float FixedDeltaTime { get { return Time.fixedDeltaTime * TimeScale(); } }
    public float DeltaTime { get { return Time.deltaTime * TimeScale(); } }
    public float TimeScale()
    {
        return TimeScaleManager.TimeScale;
    }
    public float TimeSinceLevelLoad()
    {
        return timeSinceLevelLoad;
    }
    private float timeSinceLevelLoad;

    // Update is called once per frame
    void Update()
    {
        timeSinceLevelLoad += (Time.deltaTime * TimeScale());
    }
}
