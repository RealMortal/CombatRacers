using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject MainPanel;                  // Main menu panel
    public GameObject SettingsPanel;              // Main settings menu panel
    public GameObject P1_SettingsPanel;           // Player 1 settings main panel
    public GameObject P2_SettingsPanel;           // Player 2 settings main panel
    public GameObject P1_SettingsKeyboardPanel;   // Player 1 keyboard settings panel
    public GameObject P1_SettingsControllerPanel; // Player 1 controller settings panel
    public GameObject P2_SettingsControllerPanel; // Player 2 controller settings panel

    // Called when clicking "Play" button
    public void Play()
    {
        MainPanel.SetActive(true);        // Show main panel (maybe the game scene or main menu)
        this.gameObject.SetActive(false); // Hide this menu object (probably a splash or intro)
    }

    // Called when clicking "Settings" button from the main menu
    public void Settings()
    {
        this.gameObject.SetActive(false); // Hide current panel
        SettingsPanel.SetActive(true);    // Show settings menu panel
    }

    // Called when clicking "Back" button inside the settings panel
    public void Back()
    {
        gameObject.SetActive(true);       // Show main menu again
        SettingsPanel.SetActive(false);   // Hide settings panel
    }

    // Called when clicking "Back" from Player 1 settings panel to main settings menu
    public void BackP1()
    {
        SettingsPanel.SetActive(true);    // Show main settings panel
        P1_SettingsPanel.SetActive(false);// Hide player 1 settings panel
    }

    // Called when clicking "Back" from Player 1 keyboard settings to Player 1 main settings panel
    public void BackP1_Settings()
    {
        P1_SettingsKeyboardPanel.SetActive(false); // Hide keyboard settings
        P1_SettingsPanel.SetActive(true);           // Show player 1 settings panel
    }

    // Called to open Player 1 controller settings panel
    public void Settings_Controller()
    {
        P1_SettingsControllerPanel.SetActive(true); // Show controller settings panel
        P1_SettingsPanel.SetActive(false);           // Hide player 1 main settings panel
    }

    // Called to go back from Player 1 controller settings to Player 1 main settings panel
    public void P1_Back_Settings_Controller()
    {
        P1_SettingsControllerPanel.SetActive(false); // Hide controller settings
        P1_SettingsPanel.SetActive(true);             // Show player 1 settings panel
    }

    // Called to go back from Player 2 controller settings to main settings menu
    public void BackP2_Settings()
    {
        P2_SettingsControllerPanel.SetActive(false); // Hide player 2 controller settings
        SettingsPanel.SetActive(true);                // Show main settings panel
    }

    // Called to open Player 1 keyboard settings panel
    public void P1_Keyboard_Settings()
    {
        P1_SettingsKeyboardPanel.SetActive(true);    // Show keyboard settings
        P1_SettingsPanel.SetActive(false);            // Hide player 1 main settings panel
    }

    // Called to go back from Player 2 settings panel to main settings menu
    public void BackP2()
    {
        SettingsPanel.SetActive(true);     // Show main settings panel
        P2_SettingsPanel.SetActive(false); // Hide player 2 settings panel
    }

    // Called to open Player 1 settings panel from main settings menu
    public void P1_Settings()
    {
        SettingsPanel.SetActive(false);    // Hide main settings panel
        P1_SettingsPanel.SetActive(true);  // Show player 1 settings panel
    }

    // Called to open Player 2 settings panel from main settings menu
    public void P2_Settings()
    {
        SettingsPanel.SetActive(false);    // Hide main settings panel
        P2_SettingsPanel.SetActive(true);  // Show player 2 settings panel
    }

    // Called when clicking the Quit button to close the application
    public void Quit()
    {
        Application.Quit();
    }
}
