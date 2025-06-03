using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using TMPro;

public class CameraLook : MonoBehaviour
{
    public CinemachineCamera mainCamera;
    private CinemachinePanTilt cameraTilt;

    public float sensitivity = 2f;
    public float minTilt = -10f;
    public float maxTilt = 10f;

    public TextMeshProUGUI sensetivityText;
    [SerializeField]private GameObject Player;
    private PlayerInput playerInput;
    private Vector2 lookInput;

    public void SetSensitivity(float value)
    {
        sensitivity = value;
        sensetivityText.text = value.ToString("0.0"); 
        Debug.Log($"New sensitivity: {sensitivity}");
    }

    private void Start()
    {
        playerInput = Player.GetComponent<PlayerInput>();
        playerInput.actions["Look"].performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Look"].canceled += ctx => lookInput = Vector2.zero;
        Cursor.lockState = CursorLockMode.Locked;
        cameraTilt = mainCamera.GetComponent<CinemachinePanTilt>();
    }

    public void SetPlayer(GameObject player)
    {
        Player = player;
    }

    void Update()
    {
        if (playerInput == null || Player == null) return;
        float inputX = lookInput.x * sensitivity;
        float inputY = lookInput.y * sensitivity;

        cameraTilt.PanAxis.Value += inputX * Time.deltaTime * 50f;
        cameraTilt.TiltAxis.Value = Mathf.Clamp(cameraTilt.TiltAxis.Value - (inputY * Time.deltaTime * 50f), minTilt, maxTilt);
    }


}
