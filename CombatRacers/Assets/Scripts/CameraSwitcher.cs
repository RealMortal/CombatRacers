using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera mainCamera;
    public CinemachineCamera lookBackCamera;
    public PlayerInput playerInput;
    private void Start()
    {

        playerInput.actions["SwitchCamera"].performed += ctx => SwitchCameraView();

    }

    public void SetPlayerInput(PlayerInput input)
    {
        this.playerInput = input;
    }


    private bool usingChaseCam = true;

    private void SwitchCameraView()
    {
        usingChaseCam = !usingChaseCam;

        mainCamera.Priority = usingChaseCam ? 10 : 0;
        lookBackCamera.Priority = usingChaseCam ? 0 : 10;
    }
}
