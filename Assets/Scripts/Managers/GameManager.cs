using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool initiated;

    // Update is called once per frame
    void Update()
    {
        CheckPlayerDeleted();
        initiated = true;
    }

    void CheckPlayerDeleted()
    {
        if (initiated && PlayerCharacter.Instance == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
