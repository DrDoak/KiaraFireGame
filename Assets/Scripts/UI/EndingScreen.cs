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
    private TextMeshProUGUI percentageCompletion;

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
        bool clearedAllEnemies = GameManager.Instance.defeatedEnemies == GameManager.Instance.totalEnemies;
        stageNameText.text = GameManager.Instance.stageName;
        totalEnemiesText.text = GameManager.Instance.totalEnemies.ToString();
        currentEnemiesText.text = GameManager.Instance.defeatedEnemies.ToString();
        timeComplete.text = $"{GameManager.Instance.timeElapsed.ToString("F2")} sec";
        rankingText.text = GetRank(GameManager.Instance.ranksThreasholds,GameManager.Instance.timeElapsed, clearedAllEnemies);

        percentageCompletion.text = $"{Mathf.RoundToInt((GameManager.Instance.defeatedEnemies * 100f) / GameManager.Instance.totalEnemies)} %";
        UpdateRecords(GameManager.Instance.timeElapsed, clearedAllEnemies);
    }

    private string GetRank(List<float> requirementTimes, float timeElapsed, bool clearedAllEnemies)
    {
        List<string> rankNames = clearedAllEnemies ? allKillRanks : standardRanks;
        for (int i = 0; i < requirementTimes.Count; i++)
        {
            if (timeElapsed < requirementTimes[i])
            {
                return rankNames[i];
            }
        }
        return rankNames[rankNames.Count - 1];
    }

    private void UpdateRecords(float time, bool clearedAllEnemies)
    {
        string bestTimeKey = SceneManager.GetActiveScene().name;
        if (clearedAllEnemies)
        {
            bestTimeKey += "-100";
        }
        float oldRecord = 9999999;
        if (PlayerPrefs.HasKey(bestTimeKey))
        {
            oldRecord = PlayerPrefs.GetFloat(bestTimeKey);
        }
        if (time < oldRecord)
        {
            newRecordLabel.text = "New Record";
            string bestTimeText = clearedAllEnemies ? "Best(100%): " : "Best: ";
            PlayerPrefs.SetFloat(bestTimeKey, time);
            if (oldRecord < 900)
            {
                bestTimeText += $"{oldRecord.ToString("F2")} sec";
            } else
            {
                bestTimeText += "N/A";
            }
            recordTime.text = bestTimeText;
        } else
        {
            newRecordLabel.text = "";
            recordTime.text = $"Best: {oldRecord.ToString("F2")} sec";
        }
    }
}
