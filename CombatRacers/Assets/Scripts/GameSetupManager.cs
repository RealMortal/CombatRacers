using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameSetupManager : MonoBehaviour
{
  
    [Header("Optional Speedometers")]
    public Speedometer p1Speedometer;
    public Speedometer p2Speedometer;

    [Header("Car Prefabs")]
    public GameObject[] player1CarPrefabs;
    public GameObject[] player2CarPrefabs;

    [Header("Spawn Points")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [Header("Player 1 References")]
    public RawImage p1PowerUpImage;
    public TMPro.TextMeshProUGUI p1ProgressText;
    public TMPro.TextMeshProUGUI p1LapText;
    public Image p1ParryCD;
    public Image p1HealthBar;
    public CinemachineCamera p1Camera;

    [Header("Player 2 References")]
    public RawImage p2PowerUpImage;
    public TMPro.TextMeshProUGUI p2ProgressText;
    public TMPro.TextMeshProUGUI p2LapText;
    public Image p2ParryCD;
    public Image p2HealthBar;
    public CinemachineCamera p2Camera;
    [Header("Shared")]
    public CheckPoint[] sharedCheckpoints;
    public ParticleSystem explosion;

    CameraManager cameraManager;
    public GameStartCountdown gameStartCountdown;
    public PauseMenu pauseManager;

    public GameObject endGamePanel;
    public TextMeshProUGUI playerText;
    private int P1_LapCount;
    private int P2_LapCount;
    GameObject p1Car;
    GameObject p2Car;

    public int maxLaps = 1;

    void Start()
    {
        cameraManager = GetComponent<CameraManager>();
        int p1Index = PlayerPrefs.GetInt("carIndex_P1", 0);
        int p2Index = PlayerPrefs.GetInt("carIndex_P2", 0);
        Quaternion rotatedY = Quaternion.Euler(0, 90, 0);

        p1Car = Instantiate(player1CarPrefabs[p1Index], player1Spawn.position, rotatedY);
        cameraManager.Setup(1, p1Car, p1Car.GetComponent<PlayerInput>());
        p1Speedometer?.SetTarget(p1Car.GetComponent<Rigidbody>());
        
        p2Car = Instantiate(player2CarPrefabs[p2Index], player2Spawn.position, rotatedY);
        cameraManager.Setup(2, p2Car, p2Car.GetComponent<PlayerInput>());

        p2Speedometer?.SetTarget(p2Car.GetComponent<Rigidbody>());

        CarInitializer p1Init = p1Car.GetComponent<CarInitializer>();
        if (p1Init != null)
        {
            p1Init.Initialize(
                p1PowerUpImage,
                p1Car.GetComponent<NewCarController>(),
                p1ProgressText,
                p1LapText,
                sharedCheckpoints,
                p1ParryCD,
                p1HealthBar,p1Camera, explosion
            );
        }

        // Initialize Player 2
        CarInitializer p2Init = p2Car.GetComponent<CarInitializer>();
        if (p2Init != null)
        {
            p2Init.Initialize(
                p2PowerUpImage,
                p2Car.GetComponent<NewCarController>(),
                p2ProgressText,
                p2LapText,
                sharedCheckpoints,
                p2ParryCD,
                p2HealthBar,p2Camera, explosion
            );
        }

        pauseManager.SetPlayerInput(p1Car.GetComponent<PlayerInput>(), p2Car.GetComponent<PlayerInput>());
        gameStartCountdown.SetPlayersToFreeze(p1Car, p2Car);
        gameStartCountdown.FreezePlayers();
    }

    private bool gameEnded = false;

    private void Update()
    {
        if (gameEnded) return;

        P1_LapCount = p1Car.GetComponent<LapManager>().lapCount;
        P2_LapCount = p2Car.GetComponent<LapManager>().lapCount;

        if (P1_LapCount == maxLaps)
        {
            gameEnded = true;
            playerText.text = "Player 1 Won! Congratulations";
            endGamePanel.SetActive(true);
            Time.timeScale = 0.0f;
            p1Car.GetComponent<PlayerInput>().DeactivateInput();
            Cursor.lockState = CursorLockMode.None;
        }
        else if (P2_LapCount == maxLaps)
        {
            gameEnded = true;
            playerText.text = "Player 2 Won! Congratulations";
            endGamePanel.SetActive(true);
            Time.timeScale = 0.0f;
            p2Car.GetComponent<PlayerInput>().DeactivateInput();
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
