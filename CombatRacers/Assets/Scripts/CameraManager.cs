using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages camera rigs for multiple players.
/// This script solves the problem of transferring player data (like car selection) from a separate menu scene.
/// Players are initialized at the start of the scene, and their camera rigs are linked dynamically.
///
/// Player 1 and Player 2 have separate Cinemachine cameras for default view and look-back functionality.
/// </summary>
public class CameraManager : MonoBehaviour
{
    [Header("Player 1 Camera Rig")]
    public CinemachineCamera p1DefaultCam;        // Main forward-facing camera for Player 1
    public CinemachineCamera p1LookBackCam;       // Rear-facing (look-back) camera for Player 1
    public CameraLook p1CameraLook;               // Mouse/stick look script for Player 1
    public CameraSwitcher p1CameraSwitcher;       // Handles camera toggling (e.g., between front/back)

    [Header("Player 2 Camera Rig")]
    public CinemachineCamera p2DefaultCam;        // Main forward-facing camera for Player 2
    public CinemachineCamera p2LookBackCam;       // Rear-facing camera for Player 2
    public CameraLook p2CameraLook;               // Look control for Player 2
    public CameraSwitcher p2CameraSwitcher;       // Camera switch logic for Player 2

    /// <summary>
    /// Sets up the camera rig for the given player.
    /// Called during scene initialization when spawning or loading players from the menu.
    /// </summary>
    /// <param name="playerNumber">1 or 2, indicating which camera rig to set up</param>
    /// <param name="followTarget">The player's car or character transform</param>
    /// <param name="playerInput">The associated PlayerInput component</param>
    public void Setup(int playerNumber, GameObject followTarget, PlayerInput playerInput)
    {
        if (playerNumber == 1)
        {
            // Assign camera targets for Player 1
            p1DefaultCam.Follow = followTarget.transform;
            p1DefaultCam.LookAt = followTarget.transform;
            p1LookBackCam.Follow = followTarget.transform;
            p1LookBackCam.LookAt = followTarget.transform;

            // Assign player references to control logic
            p1CameraLook.SetPlayer(followTarget);
            p1CameraSwitcher.SetPlayerInput(playerInput);
        }
        else if (playerNumber == 2)
        {
            // Assign camera targets for Player 2
            p2DefaultCam.Follow = followTarget.transform;
            p2DefaultCam.LookAt = followTarget.transform;
            p2LookBackCam.Follow = followTarget.transform;
            p2LookBackCam.LookAt = followTarget.transform;

            // Assign player references to control logic
            p2CameraLook.SetPlayer(followTarget);
            p2CameraSwitcher.SetPlayerInput(playerInput);
        }
    }
}
