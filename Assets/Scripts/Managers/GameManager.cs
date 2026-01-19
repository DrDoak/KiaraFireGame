using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool initiated;
    [SerializeField]
    private Animator mAnimator;
    [SerializeField]
    private string stageName;

    private string nextStage;
    private bool completed;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        mAnimator.Play("intro");
    }
    // Update is called once per frame
    void Update()
    {
        CheckPlayerDeleted();
        initiated = true;
        if (completed == true && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(nextStage);
        }
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
        Instance.mAnimator.Play("completed");
        Instance.nextStage = nextStageName;
        Instance.completed = true;
    }
}
