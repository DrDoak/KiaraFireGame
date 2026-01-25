using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
public class EndingScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stageNameText;
    [SerializeField]
    private TextMeshProUGUI timeComplete;
    [SerializeField]
    private TextMeshProUGUI rankingText;
    [SerializeField]
    private TextMeshProUGUI totalEnemiesText;
    [SerializeField]
    private TextMeshProUGUI currentEnemiesText;
    [SerializeField]
    private TextMeshProUGUI recordTime;
    [SerializeField]
    private TextMeshProUGUI newRecordLabel;
    [SerializeField]
    private List<string> standardRanks;

    [SerializeField]
    private List<string> allKillRanks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateEndingText()
    {

        //stageNameText.text = stageName;
    }

    public void CompletedStage()
    {
        stageNameText.text = GameManager.Instance.stageName;
        totalEnemiesText.text = GameManager.Instance.totalEnemies.ToString();
        currentEnemiesText.text = GameManager.Instance.defeatedEnemies.ToString();
        timeComplete.text = $"{GameManager.Instance.timeElapsed.ToString("F2")} sec";
        rankingText.text = GetRank(GameManager.Instance.ranksThreasholds,GameManager.Instance.timeElapsed,GameManager.Instance.defeatedEnemies, GameManager.Instance.totalEnemies);
        UpdateRecords(GameManager.Instance.timeElapsed, GameManager.Instance.defeatedEnemies, GameManager.Instance.totalEnemies);
    }

    private string GetRank(List<float> requirementTimes, float timeElapsed, int enemiesKilled, int totalEnemies)
    {
        List<string> rankNames = enemiesKilled >= totalEnemies ? allKillRanks : standardRanks;
        for (int i = 0; i < requirementTimes.Count; i++)
        {
            if (timeElapsed < requirementTimes[i])
            {
                return rankNames[i];
            }
        }
        return rankNames[rankNames.Count - 1];
    }

    private void UpdateRecords(float time, int enemiesKilled, int totalEnemies)
    {
        if (enemiesKilled < totalEnemies)
        {
            recordTime.text = "";
            newRecordLabel.text = "";
            return;
        }
        float oldRecord = 9999999;
        if (PlayerPrefs.HasKey(SceneManager.GetActiveScene().name))
        {
            oldRecord = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name);
        }
        if (time < oldRecord)
        {
            newRecordLabel.text = "New Record";
            PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name, time);
            if (oldRecord < 900)
            {
                recordTime.text = $"Best: {oldRecord.ToString("F2")} sec";
            } else
            {
                recordTime.text = "Best: N/A";
            }
            
        } else
        {
            newRecordLabel.text = "";
            recordTime.text = $"Best: {oldRecord.ToString("F2")} sec";
        }
    }
}
