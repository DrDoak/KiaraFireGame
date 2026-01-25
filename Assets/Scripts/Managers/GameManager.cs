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
    public string stageName;
    [SerializeField]
    private TextMeshProUGUI introStageName;
    [SerializeField]
    private TextMeshProUGUI pauseStageNamText;
    [SerializeField]
    private TextMeshProUGUI pauseTotalEnemiesText;
    [SerializeField]
    private TextMeshProUGUI pauseCurrentEnemiesText;
    

    [SerializeField]
    private TextMeshProUGUI timePause;
    [SerializeField]
    public List<float> ranksThreasholds;
    [SerializeField]
    private EndingScreen endingScreen;

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
        introStageName.text = stageName;
        pauseStageNamText.text = stageName;
        mAnimator.Play("intro");
        
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
        if (paused && Input.GetKeyDown(KeyCode.M))
        {
            ReturnToMenu();
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
        timePause.text = $"{timeSoFar.ToString("F2")} sec";
    }
    public void UnPause()
    {
        mAnimator.Play("unpause");
        Time.timeScale = 1;
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }
    void CheckPlayerDeleted()
    {
        if (initiated && PlayerCharacter.Instance == null && SceneManager.GetActiveScene().name != "Finish")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public static void CompletedStage(string nextstage)
    {
        Instance.mAnimator.Play("complete");
        Instance.nextStage = nextstage;
        Instance.completed = true;
        Time.timeScale = 0;
        Instance.totalTime = Instance.timeElapsed;
        Instance.endingScreen.CompletedStage();
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "-complete", 1);
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "-all-enemies",(Instance.defeatedEnemies >= Instance.totalEnemies) ? 1 : 0);
    }
}
