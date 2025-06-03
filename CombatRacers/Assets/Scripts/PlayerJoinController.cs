using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(PlayerInput))]
public class PlayerOneJoin : MonoBehaviour
{
    private PlayerInput playerInput;
    private bool usingGamepad = false;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found!");
            return;
        }

        AssignControlScheme();

        InputSystem.onDeviceChange += OnDeviceChange;

        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void AssignControlScheme()
    {
        var gamepads = Gamepad.all;

        if (gamepads.Count >= 2)
        {
            Debug.Log("[INFO] Two gamepads detected. Assigning second gamepad to Player 1.");
            playerInput.SwitchCurrentControlScheme("Gamepad", gamepads[1]);
            usingGamepad = true;
        }
        else
        {
            Debug.Log("[INFO] Using keyboard & mouse for Player 1.");
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            usingGamepad = false;
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added)
            {
                Debug.Log($"[DEVICE] Gamepad added. Re-evaluating input for Player 1.");
                if (!usingGamepad && Gamepad.all.Count >= 2)
                {
                    AssignControlScheme();
                }
            }
            else if (change == InputDeviceChange.Removed)
            {
                Debug.Log($"[DEVICE] Gamepad removed. Re-evaluating input for Player 1.");
                if (usingGamepad && Gamepad.all.Count < 2)
                {
                    AssignControlScheme(); 
                }
            }
        }
    }

    private void OnControlsChanged(PlayerInput input)
    {
        Debug.Log($"[CHANGE] Player 1 scheme changed to: {input.currentControlScheme}");
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}
