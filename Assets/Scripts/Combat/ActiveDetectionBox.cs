using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveDetectionBox : MonoBehaviour
{
    [SerializeField]
    private AnimatorOptions mAnimator;
    private Dictionary<Attackable, float> timeHit = new Dictionary<Attackable, float>();
    public float refreshTime = 0.1f;
    [SerializeField]
    private CharacterComponents components;
    private PlayerCharacter mPlayer;
    private void Start()
    {
        mPlayer = components.GetComponent<PlayerCharacter>();
    }

    public void SetActive(bool isActive)
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision == null) return;
        if (collision.gameObject == null) return;
        Hurtbox hb = collision.gameObject.GetComponent<Hurtbox>();
        if (hb == null) return;
        if (!CanAttack(hb.ParentAttackable)) return;
        TriggerAction(hb);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject == null) return;
        Hurtbox hb = collision.gameObject.GetComponent<Hurtbox>();
        if (hb == null) return;
        if (!CanAttack(hb.ParentAttackable)) return;
        TriggerAction(hb);
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
        }
    }
    private void TriggerAction(Hurtbox hb)
    {
        Attackable other = hb.ParentAttackable;
        if (components.MMovement.Grounded())
        {
            if (Input.GetKey(KeyCode.W))
            {
                components.MAnimatorOptions.PerformActionAnimation("atk_ground_up");
            } else
            {
                mPlayer.AlternateSlash();
                components.MAnimatorOptions.PerformActionAnimation("atk_ground");
            }
        } else
        {
            if (Input.GetKey(KeyCode.W))
            {
                components.MAnimatorOptions.PerformActionAnimation("atk_air_up");
            }
            else if (Input.GetKey(KeyCode.S))
            {
                components.MAnimatorOptions.PerformActionAnimation("atk_air_down");
            }
            else
            {
                mPlayer.AlternateSlash();
            }
        }
        timeHit[other] = components.MScalableTime.TimeSinceLevelLoad();
    }
    private bool CanAttack(Attackable opponent)
    {
        if (!gameObject.activeSelf) return false;
        if (timeHit.ContainsKey(opponent) && components.MScalableTime.TimeSinceLevelLoad() - timeHit[opponent] < refreshTime) return false;
        if (opponent.MyFaction == Faction.INVINCIBLE)
        {
            return false;
        }
        if (components.MAttackable.MyFaction == Faction.NEUTRAL)
        {
            return true;
        }
        return components.MAttackable.MyFaction != opponent.MyFaction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.0f, 0.4f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
