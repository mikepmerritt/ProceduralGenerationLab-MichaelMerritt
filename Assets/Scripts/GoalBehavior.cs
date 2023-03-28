using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalBehavior : MonoBehaviour
{
    void OnTriggerEnter()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string newScene = "Level" + (int.Parse(currentScene.Substring(5, 1)) + 1);
        SceneManager.LoadScene(newScene);
    }

    void Update()
    {
        // skip to next scene with enter
        if(Input.GetKeyDown(KeyCode.Return))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            string newScene = "Level" + (int.Parse(currentScene.Substring(5, 1)) + 1);
            SceneManager.LoadScene(newScene);
        }
    }
}
