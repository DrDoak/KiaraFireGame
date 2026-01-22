using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool initiated;
    [SerializeField]
    private Animator mAnimator;
    [SerializeField]
    private string stageName;
    [SerializeField]
    private TextMeshProUGUI stageNameText;
    [SerializeField]
    private TextMeshProUGUI pauseStageNamText;

    [SerializeField]
    private TextMeshProUGUI totalEnemiesText;
    [SerializeField]
    private TextMeshProUGUI pauseTotalEnemiesText;

    [SerializeField]
    private TextMeshProUGUI currentEnemiesText;
    [SerializeField]
    private TextMeshProUGUI pauseCurrentEnemiesText;

    [SerializeField]
    private TextMeshProUGUI timeComplete;
    [SerializeField]
    private TextMeshProUGUI timePause;

    private string nextStage;
    private bool completed;
    private bool paused = false;

    public int totalEnemies;
    public int defeatedEnemies;
    public float timeElapsed;
    private float totalTime;
    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }
    private void Start()
    {
        Time.timeScale = 1;
        timeElapsed = 0;
        mAnimator.Play("intro");
        stageNameText.text = stageName;
        
    }
    public static void RegisterEnemyTarget()
    {
        Instance.totalEnemies++;
    }
    public static void RegisterAsDefeated()
    {
        Instance.defeatedEnemies++;
        
    }
    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        CheckPlayerDeleted();
        initiated = true;
        if (completed == true && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(nextStage);
        }
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    void TogglePause()
    {
        if (!paused)
        {
            Pause();
        } else
        {
            UnPause();
        }
        paused = !paused;
    }
    public void Pause()
    {
        mAnimator.Play("pause");
        Time.timeScale = 0;
        float timeSoFar = timeElapsed;
        pauseTotalEnemiesText.text = totalEnemies.ToString();
        pauseCurrentEnemiesText.text = defeatedEnemies.ToString();
        timePause.text = $"{Mathf.FloorToInt(timeSoFar/60)} Min {Mathf.FloorToInt(timeSoFar % 60)} Sec";
    }
    public void UnPause()
    {
        mAnimator.Play("unpause");
        Time.timeScale = 1;
    }
    void CheckPlayerDeleted()
    {
        if (initiated && PlayerCharacter.Instance == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public static void CompletedStage(string nextStageName)
    {
        Instance.totalTime = Instance.timeElapsed;
        Instance.totalEnemiesText.text = Instance.totalEnemies.ToString();
        Instance.currentEnemiesText.text = Instance.defeatedEnemies.ToString();
        Instance.mAnimator.Play("complete");
        Instance.nextStage = nextStageName;
        Instance.completed = true;
        Time.timeScale = 0;
        Instance.timeComplete.text = $"{Mathf.FloorToInt(Instance.totalTime / 60)} Min {Mathf.FloorToInt(Instance.totalTime % 60)} Sec";
    }
}
