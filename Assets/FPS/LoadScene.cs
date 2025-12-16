using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string Prolog;        // Scene game
    public string Gallery;      // Scene gallery
    public string MainMenu;   // Scene main menu

    public void PlayGame()
    {
        SceneManager.LoadScene(Prolog);
    }

    public void OpenGallery()
    {
        SceneManager.LoadScene(Gallery);
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene(MainMenu);
    }
}
