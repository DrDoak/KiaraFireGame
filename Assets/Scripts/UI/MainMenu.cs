using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI bestTime;
    [SerializeField]
    private GameObject clearedText;
    [SerializeField]
    private GameObject allEnemiesDefeated;
    [SerializeField]
    private Button startButton;
    private string sceneName = "none";
    [SerializeField]
    private EventReference startMusicEvent;
    [SerializeField]
    private List<float> EagleTimes;
    [SerializeField]
    private List<float> PhoenixTimes;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.PlayMusic(startMusicEvent);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SelectScene(string sceneName)
    {
        this.sceneName = sceneName;
        int timeIndex = 0;
        int.TryParse(sceneName.Substring(sceneName.Length - 1), out timeIndex);
        clearedText.SetActive(PlayerPrefs.HasKey(sceneName + "-complete") && PlayerPrefs.GetInt(sceneName + "-complete") == 1);
        allEnemiesDefeated.SetActive(PlayerPrefs.HasKey(sceneName + "-all-enemies") && PlayerPrefs.GetInt(sceneName + "-all-enemies") == 1);
        if (PlayerPrefs.HasKey(sceneName))
        {
            float bestTimeVar = PlayerPrefs.GetFloat(sceneName);
            bestTime.text = $"Best: {bestTimeVar.ToString("F2")} sec";
            if (bestTimeVar < PhoenixTimes[timeIndex - 1])
            {
                bestTime.text += " (P)";
            } else if (bestTimeVar < EagleTimes[timeIndex - 1])
            {
                bestTime.text += " (E)";
            }
        } else
        {
            bestTime.text = "";
        }
        GetComponent<AudioComponent>().PlaySounds("MenuClick");
        startButton.interactable = (sceneName != "none");
    }
    public void LoadSelectedScene()
    {
        if (sceneName != "none")
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ToggleObjects(GameObject obj)
    {
        GetComponent<AudioComponent>().PlaySounds("MenuClick");
        obj.SetActive(!obj.activeSelf);
        if (!obj.activeSelf)
        {
            startButton.interactable = false;
            sceneName = "none";
            SelectScene(sceneName);
        }
    }
}
