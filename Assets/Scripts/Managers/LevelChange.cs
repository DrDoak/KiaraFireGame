using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;

public class LevelChange : MonoBehaviour
{
    public string newLevel;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject == null) return;
        Hurtbox hb = collision.gameObject.GetComponent<Hurtbox>();
        if (hb == null) return;
        if (hb.ParentAttackable.GetComponent<PlayerCharacter>() != null)
        {
            SceneManager.LoadScene(newLevel);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.0f, 0.5f, 0.4f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
