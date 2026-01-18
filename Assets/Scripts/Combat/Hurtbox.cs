using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public Attackable ParentAttackable { get { return parentAttackable; } }
    [SerializeField]
    private Attackable parentAttackable;
    private CharacterComponents components;

    // Start is called before the first frame update


    public void SetParent(Attackable a)
    {
        parentAttackable = a;
        components = a.components;
    }
    public void TakeHit(Hitbox hb)
    {
        parentAttackable.TakeHit(hb, this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 0.0f, 0.5f, 0.4f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
