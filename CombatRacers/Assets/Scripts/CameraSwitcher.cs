using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Switches between the main chase camera and the look-back camera based on player input.
/// Designed for a multiplayer racing game using Unity Input System and Cinemachine.
/// </summary>
public class CameraSwitcher : MonoBehaviour
{
    // Forward-facing camera (chase cam)
    public CinemachineCamera mainCamera;

    // Rear-facing camera (look-back cam)
    public CinemachineCamera lookBackCamera;

    // Input component for the player this switcher belongs to
    public PlayerInput playerInput;

    // Tracks which camera is currently active
    private bool usingChaseCam = true;

    private void Start()
    {
        // Register input callback for camera switching
        playerInput.actions["SwitchCamera"].performed += ctx => SwitchCameraView();
    }

    /// <summary>
    /// Assigns the input system reference for the current player.
    /// Called during runtime initialization by the CameraManager.
    /// </summary>
    public void SetPlayerInput(PlayerInput input)
    {
        this.playerInput = input;
    }

    /// <summary>
    /// Switches between chase and look-back cameras by adjusting their priorities.
    /// </summary>
    private void SwitchCameraView()
    {
        usingChaseCam = !usingChaseCam;

        // Cinemachine selects the camera with the highest priority
        mainCamera.Priority = usingChaseCam ? 10 : 0;
        lookBackCamera.Priority = usingChaseCam ? 0 : 10;
    }
}
