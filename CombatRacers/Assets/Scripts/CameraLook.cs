using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Controls camera look direction based on player input using Cinemachine.
/// Includes support for adjustable sensitivity and tilt clamping.
/// </summary>
public class CameraLook : MonoBehaviour
{
    // Reference to the Cinemachine camera to control
    public CinemachineCamera mainCamera;

    // Internal reference to Cinemachine's Pan/Tilt component
    private CinemachinePanTilt cameraTilt;

    // Mouse look sensitivity (modifiable via UI)
    public float sensitivity = 2f;

    // Tilt angle limits to avoid over-rotation
    public float minTilt = -10f;
    public float maxTilt = 10f;

    // UI text to display current sensitivity value
    public TextMeshProUGUI sensetivityText;

    // Reference to the player GameObject (provides input actions)
    [SerializeField] private GameObject Player;

    // Cached reference to the player's input component
    private PlayerInput playerInput;

    // Vector storing real-time look input (mouse or stick)
    private Vector2 lookInput;

    /// <summary>
    /// Called by a UI slider to set the sensitivity dynamically.
    /// Updates both the internal value and on-screen display.
    /// </summary>
    public void SetSensitivity(float value)
    {
        sensitivity = value;
        sensetivityText.text = value.ToString("0.0"); 
        Debug.Log($"New sensitivity: {sensitivity}");
    }

    private void Start()
    {
        // Get the PlayerInput component from the assigned player
        playerInput = Player.GetComponent<PlayerInput>();

        // Bind the Look action to update the lookInput vector when performed
        playerInput.actions["Look"].performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Look"].canceled += ctx => lookInput = Vector2.zero;

        // Lock the mouse cursor for a typical FPS-style look system
        Cursor.lockState = CursorLockMode.Locked;

        // Get the CinemachinePanTilt component to manipulate camera rotation
        cameraTilt = mainCamera.GetComponent<CinemachinePanTilt>();
    }

    /// <summary>
    /// Allows assignment of a new player GameObject at runtime.
    /// Useful for character switching or dynamic spawning.
    /// </summary>
    public void SetPlayer(GameObject player)
    {
        Player = player;
    }

    private void Update()
    {
        // Safety check to avoid null references
        if (playerInput == null || Player == null) return;

        // Scale input by sensitivity
        float inputX = lookInput.x * sensitivity;
        float inputY = lookInput.y * sensitivity;

        // Apply horizontal rotation (pan)
        cameraTilt.PanAxis.Value += inputX * Time.deltaTime * 50f;

        // Apply vertical rotation (tilt), clamped to limits
        cameraTilt.TiltAxis.Value = Mathf.Clamp(
            cameraTilt.TiltAxis.Value - (inputY * Time.deltaTime * 50f),
            minTilt,
            maxTilt
        );
    }
}
