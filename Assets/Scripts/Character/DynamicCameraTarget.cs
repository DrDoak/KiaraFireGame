using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCameraTarget : MonoBehaviour
{
    private Vector3 currentTarget;
    private Vector3 defaultTarget;
    [SerializeField]
    private float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = transform.localPosition;
        defaultTarget = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }

    public void SetNewTarget(Vector3 newTarget)
    {
        currentTarget = newTarget;
    }
    public void ResetTarget()
    {
        currentTarget = defaultTarget;
    }
    void MoveToTarget()
    {
        if (Vector3.Distance(transform.localPosition,currentTarget) < 0.2f)
        {
            float angle = Mathf.Atan2(currentTarget.y - transform.localPosition.y, currentTarget.x - transform.localPosition.x);
            Vector3 rawSpeed = new Vector3(Mathf.Cos(angle) * moveSpeed, Mathf.Sin(angle) * moveSpeed, 0);
            transform.localPosition += (rawSpeed * Time.deltaTime);
        } else
        {
            transform.localPosition = currentTarget;
        }
    }
}
