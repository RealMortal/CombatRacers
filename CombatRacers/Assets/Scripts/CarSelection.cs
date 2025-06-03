using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarSelection : MonoBehaviour
{
    [Header("Car References")]
    public GameObject[] P1_cars;
    public GameObject[] P2_cars;

    [Header("UI Panels")]
    public GameObject MainPanel;
    public GameObject P1_UI;
    public GameObject P2_UI;
    public GameObject MainMenu;

    [Header("Buttons")]
    public Button next;
    public Button prev;
    public Button nextPlayer;    // "Switch" button
    public Button backToP1;
    public Button startGame;

    private int index = 0;
    private int playerNumber = 1; // 1 = Player 1, 2 = Player 2
    private GameObject[] currentCars;

    void Start()
    {
        playerNumber = 1;
        currentCars = P1_cars;
        index = PlayerPrefs.GetInt("carIndex_P1", 0);

        DeactivateAllCars();
        ShowCurrentCar();

        P1_UI.SetActive(true);
        P2_UI.SetActive(false);
    }

    void Update()
    {
        next.interactable = index < currentCars.Length - 1;
        prev.interactable = index > 0;
    }

    public void Next()
    {
        if (index < currentCars.Length - 1)
        {
            index++;
            ShowCurrentCar();
            SaveSelection();
        }
    }

    public void Prev()
    {
        if (index > 0)
        {
            index--;
            ShowCurrentCar();
            SaveSelection();
        }
    }

    public void SelectNextPlayer()
    {
        SaveSelection();
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

    public void BackToMainMenu()
    {
        MainPanel.SetActive(false);
        MainMenu.SetActive(true);
    }

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

    public void StartGame()
    {
        SaveSelection();

        int selectedCarP1 = PlayerPrefs.GetInt("carIndex_P1");
        int selectedCarP2 = PlayerPrefs.GetInt("carIndex_P2");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);


    }

    private void ShowCurrentCar()
    {
        for (int i = 0; i < currentCars.Length; i++)
            currentCars[i].SetActive(i == index);
    }

    private void DeactivateAllCars()
    {
        foreach (var car in P1_cars)
            car.SetActive(false);

        foreach (var car in P2_cars)
            car.SetActive(false);
    }

    private void SaveSelection()
    {
        string key = playerNumber == 1 ? "carIndex_P1" : "carIndex_P2";
        PlayerPrefs.SetInt(key, index);
        PlayerPrefs.Save();
    }
}
