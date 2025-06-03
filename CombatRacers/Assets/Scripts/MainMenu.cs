using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [Header("UI Panels")]
    public GameObject MainPanel;
    public GameObject SettingsPanel;
    public GameObject P1_SettingsPanel;
    public GameObject P2_SettingsPanel;
    public GameObject P1_SettingsKeyboardPanel;
    public GameObject P1_SettingsControllerPanel;

    public GameObject P2_SettingsControllerPanel;

    public void Play()
    {
        MainPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void Settings()
    {
        this.gameObject.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void Back()
    {
        gameObject.SetActive(true);
        SettingsPanel.SetActive(false);
    }

    public void BackP1()
    {
        SettingsPanel.SetActive(true);
        P1_SettingsPanel.SetActive(false);
    }

    public void BackP1_Settings()
    {
        P1_SettingsKeyboardPanel.SetActive(false);
        P1_SettingsPanel.SetActive(true);
    }
    public void Settings_Controller()
    {
        P1_SettingsControllerPanel.SetActive(true);
        P1_SettingsPanel.SetActive(false);
    }

    public void P1_Back_Settings_Controller()
    {
        P1_SettingsControllerPanel.SetActive(false);
        P1_SettingsPanel.SetActive(true);
    }

    public void BackP2_Settings()
    {
        P2_SettingsControllerPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }
    public void P1_Keyboard_Settings()
    {
        P1_SettingsKeyboardPanel.SetActive(true);
        P1_SettingsPanel.SetActive(false);
    }

    public void BackP2()
    {
        SettingsPanel.SetActive(true);
        P2_SettingsPanel.SetActive(false);
    }


    public void P1_Settings()
    {
        SettingsPanel.SetActive(false);
        P1_SettingsPanel.SetActive(true);
    }

    public void P2_Settings()
    {
        SettingsPanel.SetActive(false);
        P2_SettingsPanel.SetActive(true);
    }
    public void Quit()
    {
        Application.Quit();
    }
}

