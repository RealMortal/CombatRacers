using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages car selection for two players in the car selection menu.
/// Supports switching between Player 1 and Player 2 car choices, navigation through car options,
/// saving selections using PlayerPrefs, and transitioning to the next scene.
/// </summary>
public class CarSelection : MonoBehaviour
{
    [Header("Car References")]
    public GameObject[] P1_cars;      // Array of Player 1's car game objects
    public GameObject[] P2_cars;      // Array of Player 2's car game objects

    [Header("UI Panels")]
    public GameObject MainPanel;      // Panel that contains the car selection UI
    public GameObject P1_UI;          // Player 1 specific UI panel
    public GameObject P2_UI;          // Player 2 specific UI panel
    public GameObject MainMenu;       // Main menu panel to return to

    [Header("Buttons")]
    public Button next;               // Button to cycle to next car
    public Button prev;               // Button to cycle to previous car
    public Button nextPlayer;         // Button to switch from Player 1 to Player 2 selection
    public Button backToP1;           // Button to switch back to Player 1 selection
    public Button startGame;          // Button to start the game

    private int index = 0;            // Current selected car index in the array
    private int playerNumber = 1;     // Tracks current player (1 or 2)
    private GameObject[] currentCars; // Reference to the currently active player's car array

    private void Start()
    {
        playerNumber = 1;
        currentCars = P1_cars;

        // Load previously saved car selection if exists
        index = PlayerPrefs.GetInt("carIndex_P1", 0);

        DeactivateAllCars();   // Make sure no cars are active before showing selected
        ShowCurrentCar();      // Show currently selected car for player 1

        P1_UI.SetActive(true);
        P2_UI.SetActive(false);
    }

    private void Update()
    {
        // Enable or disable next/prev buttons based on current index to avoid out-of-range selection
        next.interactable = index < currentCars.Length - 1;
        prev.interactable = index > 0;
    }

    /// <summary>
    /// Select the next car in the current player's car list.
    /// </summary>
    public void Next()
    {
        if (index < currentCars.Length - 1)
        {
            index++;
            ShowCurrentCar();
            SaveSelection();
        }
    }

    /// <summary>
    /// Select the previous car in the current player's car list.
    /// </summary>
    public void Prev()
    {
        if (index > 0)
        {
            index--;
            ShowCurrentCar();
            SaveSelection();
        }
    }

    /// <summary>
    /// Switch from Player 1's selection to Player 2's selection.
    /// Loads saved index for Player 2 or starts at 0.
    /// Updates UI accordingly.
    /// </summary>
    public void SelectNextPlayer()
    {
        SaveSelection();   // Save current player's selection before switching
        DeactivateAllCars();

        if (playerNumber == 1)
        {
            playerNumber = 2;
            currentCars = P2_cars;
            index = PlayerPrefs.GetInt("carIndex_P2", 0);

            ShowCurrentCar();

            P1_UI.SetActive(false);
            P2_UI.SetActive(true);
        }
    }

    /// <summary>
    /// Return from car selection menu back to the main menu UI.
    /// </summary>
    public void BackToMainMenu()
    {
        MainPanel.SetActive(false);
        MainMenu.SetActive(true);
    }

    /// <summary>
    /// Switch back to Player 1's car selection from Player 2.
    /// Loads Player 1's saved index and updates UI.
    /// </summary>
    public void BackToPlayer1()
    {
        SaveSelection();
        DeactivateAllCars();

        playerNumber = 1;
        currentCars = P1_cars;
        index = PlayerPrefs.GetInt("carIndex_P1", 0);

        ShowCurrentCar();

        P1_UI.SetActive(true);
        P2_UI.SetActive(false);
    }

    /// <summary>
    /// Save current selections and load the next scene (e.g., race scene).
    /// </summary>
    public void StartGame()
    {
        SaveSelection();

        // Can retrieve selected cars here if needed:
        int selectedCarP1 = PlayerPrefs.GetInt("carIndex_P1");
        int selectedCarP2 = PlayerPrefs.GetInt("carIndex_P2");

        // Loads the next scene by build index (+1)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Activates only the currently selected car and deactivates all others.
    /// </summary>
    private void ShowCurrentCar()
    {
        for (int i = 0; i < currentCars.Length; i++)
            currentCars[i].SetActive(i == index);
    }

    /// <summary>
    /// Deactivates all cars for both players to ensure only one is visible.
    /// </summary>
    private void DeactivateAllCars()
    {
        foreach (var car in P1_cars)
            car.SetActive(false);

        foreach (var car in P2_cars)
            car.SetActive(false);
    }

    /// <summary>
    /// Saves the current selected car index for the active player using PlayerPrefs.
    /// </summary>
    private void SaveSelection()
    {
        string key = playerNumber == 1 ? "carIndex_P1" : "carIndex_P2";
        PlayerPrefs.SetInt(key, index);
        PlayerPrefs.Save();
    }
}
