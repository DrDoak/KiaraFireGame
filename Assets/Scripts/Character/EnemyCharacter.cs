using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField]
    private float characterDetectionZone;
    [SerializeField]
    private float delayUntilAttack;
    [SerializeField]
    private float delayBetweenAttacks;
    [SerializeField]
    private string attackAnimation;
    [SerializeField]
    private bool renderRange;

    private float timeFirstFoundPlayer;
    private float timeOfNextAttack;
    [SerializeField]
    private float moveToPlayerSpeed;
    [SerializeField]
    private Vector2 playerOffset;
    private bool foundPlayer;
    private CharacterComponents components;
    
    // Start is called before the first frame update
    void Start()
    {
        components = GetComponent<CharacterComponents>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHitStun > 0)
        {
            DecreaseHitStun();
        }
        if (!EnemyWithinRange()) return;
        components.MMovement.SetFacingLeft(PlayerCharacter.Instance.transform.position.x < transform.position.x);
        if (!foundPlayer) {
            foundPlayer = true;
            timeFirstFoundPlayer = components.MScalableTime.TimeSinceLevelLoad();
            timeOfNextAttack = components.MScalableTime.TimeSinceLevelLoad() + delayUntilAttack;
        }
        if (components.MScalableTime.TimeSinceLevelLoad() > timeOfNextAttack)
        {
            timeOfNextAttack = components.MScalableTime.TimeSinceLevelLoad() + delayBetweenAttacks;
            components.MAnimatorOptions.PlayAnimation(attackAnimation);
        }
        if (moveToPlayerSpeed > 0)
        {
            MoveToPlayer();
        }
    }
    private bool EnemyWithinRange()
    {
        if (PlayerCharacter.Instance == null) return false;
        Vector2 theirDist = new Vector2(PlayerCharacter.Instance.transform.position.x, PlayerCharacter.Instance.transform.position.y);
        Vector2 myDist = new Vector2(transform.position.x, transform.position.y);

        float dist = Vector2.Distance(theirDist, myDist);
        return dist < characterDetectionZone;
    }
    private void MoveToPlayer()
    {
        Vector2 targetPoint = new Vector2(PlayerCharacter.Instance.transform.position.x + playerOffset.x, PlayerCharacter.Instance.transform.position.y + playerOffset.y);
        float angle = Mathf.Atan2(targetPoint.y - transform.position.y, targetPoint.x - transform.position.x);
        Vector2 rawSpeed = new Vector2(Mathf.Cos(angle) * moveToPlayerSpeed, Mathf.Sin(angle) * moveToPlayerSpeed);
        components.MMovement.SetPersonalVelocity(rawSpeed);
    }
    private void OnDrawGizmos()
    {
        if (renderRange)
        {
            Gizmos.color = new Color(0.5f, 0.0f, 0.0f, 0.4f);
            Gizmos.DrawSphere(transform.position, characterDetectionZone);
        }
    }

}
