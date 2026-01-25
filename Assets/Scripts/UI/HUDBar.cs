using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HUDBar : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem swordBurn;
    [SerializeField]
    private ParticleSystem swordEdgeBurn;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private TextMeshProUGUI enemyText;
    [SerializeField]
    private TextMeshProUGUI timeText;

    private Attackable playerAttackable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     if (PlayerCharacter.Instance != null)
        {
            UpdatePlayer();
            UpdateTimers();
        }   
    }

    void UpdatePlayer()
    {
        if (playerAttackable == null) {
            playerAttackable = PlayerCharacter.Instance.GetComponent<Attackable>();
        }
        float ratio = playerAttackable.CurrentHP / playerAttackable.MaxHP;
        healthBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ratio * 900);
        if (ratio < 0.9)
        {
            swordEdgeBurn.Play();
        } else
        {
            swordEdgeBurn.Stop();
        }
        if (PlayerCharacter.Instance.HasSmallCharge())
        {
            swordBurn.Play();
        } else
        {
            swordBurn.Stop();
        }
    }

    void UpdateTimers()
    {
        float timeSoFar = GameManager.Instance.timeElapsed;
        timeText.text = $"{timeSoFar.ToString("F2")} sec";
        enemyText.text = $"{GameManager.Instance.defeatedEnemies} / {GameManager.Instance.totalEnemies}";
    }
}
