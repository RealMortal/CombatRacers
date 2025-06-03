using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject optionMenu;
    private bool inOptionsMenu = false;

    PlayerInput P1_input;
    PlayerInput P2_input;

    public void SetPlayerInput(PlayerInput p1,PlayerInput p2)
    {
        P1_input = p1;
        P2_input = p2;
    }
    
    private void Update()
    {
        if (inOptionsMenu) return; 

        if (P1_input.actions["Pause"].WasPressedThisFrame() || P2_input.actions["Pause"].WasPressedThisFrame())
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }
    

    public void Resume()
    {
        print("Resumed");
        Cursor.lockState = CursorLockMode.Locked;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        GameIsPaused = false;

    }

    void Pause()
    {
        print("Paused");
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        GameIsPaused = true;
    }

    public void LoadOptions()
    {
        print("Options");
        inOptionsMenu = true;

        pauseMenuUI.SetActive(false);
        optionMenu.SetActive(true);
       
    }

    public void Back()
    {
        print("Back");
        inOptionsMenu = false;

        pauseMenuUI.SetActive(true);
        optionMenu.SetActive(false);

    }
    public void Quit()
    {
        print("Back to main menu");
        Time.timeScale = 1.0f;
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int previousIndex = currentIndex - 1;
        SceneManager.LoadScene(previousIndex);
    }
}
