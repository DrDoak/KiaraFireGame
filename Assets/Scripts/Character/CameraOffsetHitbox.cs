using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    [SerializeField]
    private Vector2 newCameraOffset;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject == null) return;
        Hurtbox hb = collision.gameObject.GetComponent<Hurtbox>();
        if (hb == null) return;
        if (hb.ParentAttackable.components.camTarget == null) return;
        hb.ParentAttackable.components.camTarget.SetNewTarget(newCameraOffset);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject == null) return;
        Hurtbox hb = collision.gameObject.GetComponent<Hurtbox>();
        if (hb == null) return;
        if (hb.ParentAttackable.components.camTarget == null) return;
        hb.ParentAttackable.components.camTarget.ResetTarget();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.1f, 0.4f, 0.1f, 0.1f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
