using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("Player 1 Camera Rig")]
    public CinemachineCamera p1DefaultCam;
    public CinemachineCamera p1LookBackCam;
    public CameraLook p1CameraLook;
    public CameraSwitcher p1CameraSwitcher;

    [Header("Player 2 Camera Rig")]
    public CinemachineCamera p2DefaultCam;
    public CinemachineCamera p2LookBackCam;
    public CameraLook p2CameraLook;
    public CameraSwitcher p2CameraSwitcher;

 
    public void Setup(int playerNumber, GameObject followTarget, PlayerInput playerInput)
    {
        if (playerNumber == 1)
        {
            p1DefaultCam.Follow = followTarget.transform;
            p1DefaultCam.LookAt = followTarget.transform;
            p1LookBackCam.Follow = followTarget.transform;
            p1LookBackCam.LookAt = followTarget.transform;

            p1CameraLook.SetPlayer(followTarget);
            p1CameraSwitcher.SetPlayerInput(playerInput);
        }
        else if (playerNumber == 2)
        {
            p2DefaultCam.Follow = followTarget.transform;
            p2DefaultCam.LookAt = followTarget.transform;
            p2LookBackCam.Follow = followTarget.transform;
            p2LookBackCam.LookAt = followTarget.transform;

            p2CameraLook.SetPlayer(followTarget);
            p2CameraSwitcher.SetPlayerInput(playerInput);
        }
    }
}
