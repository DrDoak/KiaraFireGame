using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage = 10.0f;
    public Vector2 knockback;
    public float hitstunSeconds = 0.5f;
    public float hitstopSeconds = 0f;
    public int multiHits = 1;
    public float refreshTime = 0.15f;
    public Attackable ParentAttackable { get { return parentAttackable; } }
    [SerializeField]
    private Attackable parentAttackable;
    private CharacterComponents components;
    private Dictionary<Attackable, float> timeHit = new Dictionary<Attackable, float>();
    private Dictionary<Attackable, int> numMultiHit = new Dictionary<Attackable, int>();
  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetParent(Attackable a)
    {
        parentAttackable = a;
        components = a.components;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject == null) return;
        Hurtbox hb = collision.gameObject.GetComponent<Hurtbox>();
        if (hb == null) return;
        if (!CanAttack(hb.ParentAttackable)) return;
        TriggerHit(hb);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject == null) return;
        Hurtbox hb = collision.gameObject.GetComponent<Hurtbox>();
        if (hb == null) return;
        if (!CanAttack(hb.ParentAttackable)) return;
        TriggerHit(hb);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject == null) return;
        Hurtbox hb = collision.gameObject.GetComponent<Hurtbox>();
        if (hb == null) return;
        if (!CanAttack(hb.ParentAttackable)) return;
        if (timeHit.ContainsKey(hb.ParentAttackable))
        {
            timeHit.Remove(hb.ParentAttackable);
            numMultiHit.Remove(hb.ParentAttackable);
        }
    }
    private void TriggerHit(Hurtbox hb)
    {
        Attackable other = hb.ParentAttackable;
        hb.TakeHit(this);
        timeHit[other] = components.MScalableTime.TimeSinceLevelLoad();
        if (!numMultiHit.ContainsKey(other))
        {
            numMultiHit[other] = 0;
        }
        numMultiHit[other]++;
    }
    private bool CanAttack(Attackable opponent)
    {
        if (numMultiHit.ContainsKey(opponent) && numMultiHit[opponent] >= multiHits) return false;
        if (timeHit.ContainsKey(opponent) && components.MScalableTime.TimeSinceLevelLoad() - timeHit[opponent] < refreshTime) return false;
        if (opponent.MyFaction == Faction.INVINCIBLE)
        {
            return false;
        }
        if (parentAttackable.MyFaction == Faction.NEUTRAL)
        {
            return true;
        }
        return parentAttackable.MyFaction != opponent.MyFaction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.0f, 0.0f, 0.4f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
