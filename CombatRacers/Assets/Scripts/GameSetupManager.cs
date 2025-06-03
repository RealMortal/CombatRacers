using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameSetupManager : MonoBehaviour
{
    [Header("Optional Speedometers")]
    public Speedometer p1Speedometer;  // Speedometer UI for Player 1 (optional)
    public Speedometer p2Speedometer;  // Speedometer UI for Player 2 (optional)

    [Header("Car Prefabs")]
    public GameObject[] player1CarPrefabs;  // Available car prefabs for Player 1
    public GameObject[] player2CarPrefabs;  // Available car prefabs for Player 2

    [Header("Spawn Points")]
    public Transform player1Spawn;  // Spawn position and rotation for Player 1 car
    public Transform player2Spawn;  // Spawn position and rotation for Player 2 car

    [Header("Player 1 References")]
    public RawImage p1PowerUpImage;          // UI element to show Player 1's current power-up
    public TMPro.TextMeshProUGUI p1ProgressText; // UI for Player 1's lap progress
    public TMPro.TextMeshProUGUI p1LapText;       // UI for Player 1's lap count
    public Image p1ParryCD;                  // UI cooldown bar for Player 1's parry ability
    public Image p1HealthBar;                // UI health bar for Player 1's durability
    public CinemachineCamera p1Camera;      // Player 1’s Cinemachine camera rig

    [Header("Player 2 References")]
    public RawImage p2PowerUpImage;          // UI element to show Player 2's current power-up
    public TMPro.TextMeshProUGUI p2ProgressText; // UI for Player 2's lap progress
    public TMPro.TextMeshProUGUI p2LapText;       // UI for Player 2's lap count
    public Image p2ParryCD;                  // UI cooldown bar for Player 2's parry ability
    public Image p2HealthBar;                // UI health bar for Player 2's durability
    public CinemachineCamera p2Camera;      // Player 2’s Cinemachine camera rig

    [Header("Shared")]
    public CheckPoint[] sharedCheckpoints;  // Checkpoints used by both players
    public ParticleSystem explosion;        // Explosion particle effect to use on car destruction

    CameraManager cameraManager;             // Reference to camera manager for setting up cameras
    public GameStartCountdown gameStartCountdown;  // Controls countdown and freezing players before start
    public PauseMenu pauseManager;             // Reference to pause menu manager

    public GameObject endGamePanel;            // UI panel shown at game end
    public TextMeshProUGUI playerText;         // Text displaying the winning player message
    private int P1_LapCount;                   // Current lap count for Player 1
    private int P2_LapCount;                   // Current lap count for Player 2
    GameObject p1Car;                         // Reference to the instantiated Player 1 car
    GameObject p2Car;                         // Reference to the instantiated Player 2 car

    public int maxLaps = 1;                    // Number of laps required to win the game

    void Start()
    {
        // Get the CameraManager component attached to this GameObject
        cameraManager = GetComponent<CameraManager>();

        // Load the players' car selections saved from previous scene or default to index 0
        int p1Index = PlayerPrefs.GetInt("carIndex_P1", 0);
        int p2Index = PlayerPrefs.GetInt("carIndex_P2", 0);

        // Rotation to align cars facing the track start direction (90 degrees on Y axis)
        Quaternion rotatedY = Quaternion.Euler(0, 90, 0);

        // Instantiate Player 1's car prefab at spawn point with rotation
        p1Car = Instantiate(player1CarPrefabs[p1Index], player1Spawn.position, rotatedY);
        // Setup Player 1's cameras to follow and look at their car
        cameraManager.Setup(1, p1Car, p1Car.GetComponent<PlayerInput>());
        // Assign Rigidbody to Player 1 speedometer UI if it exists
        p1Speedometer?.SetTarget(p1Car.GetComponent<Rigidbody>());

        // Instantiate Player 2's car prefab at spawn point with rotation
        p2Car = Instantiate(player2CarPrefabs[p2Index], player2Spawn.position, rotatedY);
        // Setup Player 2's cameras to follow and look at their car
        cameraManager.Setup(2, p2Car, p2Car.GetComponent<PlayerInput>());
        // Assign Rigidbody to Player 2 speedometer UI if it exists
        p2Speedometer?.SetTarget(p2Car.GetComponent<Rigidbody>());

        // Initialize Player 1's car UI and gameplay components via CarInitializer
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
                p1HealthBar,
                p1Camera,
                explosion
            );
        }

        // Initialize Player 2's car UI and gameplay components via CarInitializer
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
                p2HealthBar,
                p2Camera,
                explosion
            );
        }

        // Setup pause manager with references to both players' input components
        pauseManager.SetPlayerInput(p1Car.GetComponent<PlayerInput>(), p2Car.GetComponent<PlayerInput>());

        // Set the players to be frozen initially during countdown
        gameStartCountdown.SetPlayersToFreeze(p1Car, p2Car);
        gameStartCountdown.FreezePlayers();
    }

    private bool gameEnded = false; // Flag to prevent running end game logic multiple times

    private void Update()
    {
        if (gameEnded) return; // Skip update if game has ended

        // Update lap counts from each player's LapManager component
        P1_LapCount = p1Car.GetComponent<LapManager>().lapCount;
        P2_LapCount = p2Car.GetComponent<LapManager>().lapCount;

        // Check if Player 1 reached max laps and trigger end game
        if (P1_LapCount == maxLaps)
        {
            gameEnded = true;
            playerText.text = "Player 1 Won! Congratulations";
            endGamePanel.SetActive(true);

            // Freeze game by setting time scale to zero
            Time.timeScale = 0.0f;

            // Disable Player 1 input to prevent further control
            p1Car.GetComponent<PlayerInput>().DeactivateInput();

            // Unlock the mouse cursor for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        // Check if Player 2 reached max laps and trigger end game
        else if (P2_LapCount == maxLaps)
        {
            gameEnded = true;
            playerText.text = "Player 2 Won! Congratulations";
            endGamePanel.SetActive(true);

            // Freeze game by setting time scale to zero
            Time.timeScale = 0.0f;

            // Disable Player 2 input to prevent further control
            p2Car.GetComponent<PlayerInput>().DeactivateInput();

            // Unlock the mouse cursor for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
