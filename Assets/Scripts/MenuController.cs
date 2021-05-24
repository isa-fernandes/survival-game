using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject instructionsPanel;
    [SerializeField] GameObject menuPanel;

    public void StartGame ()
    {
        SceneManager.LoadScene(SceneNames.Level);
    }

    public void Quit()
    {
        Application.Quit(); 
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    public void Pause ()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
    }

    public void Resume ()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
    }

    public void Restart ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneNames.Menu);
    }

    public void Instructions ()
    {
        instructionsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void GoBackMenu ()
    {
        instructionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
