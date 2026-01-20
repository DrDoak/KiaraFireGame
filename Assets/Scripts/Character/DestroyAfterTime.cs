using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float destroyTime;
    private float expirationDate;
    // Start is called before the first frame update
    void Start()
    {
        expirationDate = Time.timeSinceLevelLoad + destroyTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > expirationDate)
        {
            Destroy(gameObject);
        }
    }
}
