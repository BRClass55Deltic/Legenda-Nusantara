using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePindah : MonoBehaviour
{
    // Pindah scene berdasarkan nama
    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
       
        SceneManager.LoadScene(sceneName);
        Debug.Log("You PLAY");
    }

    // Keluar game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("YOU EXIT THE GAME");
    }
}