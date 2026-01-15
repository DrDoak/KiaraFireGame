using UnityEngine;
using System.Linq;

public class SurroundingSensors : MonoBehaviour
{
    [Header("Ceiling Check")]
    [SerializeField] private Collider2D CeilingCheck;
    [SerializeField] private LayerMask CeilingLayer;
    public bool HitCeiling { get; private set; }

    [Header("Ground Check")]
    [SerializeField] private Collider2D GroundCheck;
    [SerializeField] private LayerMask GroundLayer;
    public bool Grounded { get; private set; }

    private void FixedUpdate()
    {
        CheckCeiling();
        CheckGround();
    }

    private void CheckCeiling()
    {
        HitCeiling = Physics2D.OverlapAreaAll(CeilingCheck.bounds.min, CeilingCheck.bounds.max, CeilingLayer).Any(collider => !collider.isTrigger);
    }

    private void CheckGround()
    {
        Grounded = Physics2D.OverlapAreaAll(GroundCheck.bounds.min, GroundCheck.bounds.max, GroundLayer).Any(collider => !collider.isTrigger);
    }
}