using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

// Require PlayerInput component on the same GameObject
[RequireComponent(typeof(PlayerInput))]
public class PlayerOneJoin : MonoBehaviour
{
    private PlayerInput playerInput;  // Reference to PlayerInput component
    private bool usingGamepad = false;  // Tracks if player is currently using a gamepad

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found!");
            return;
        }

        // Assign initial control scheme based on connected devices
        AssignControlScheme();

        // Subscribe to device connection/disconnection events
        InputSystem.onDeviceChange += OnDeviceChange;

        // Subscribe to control scheme change events (e.g., switching keyboard to gamepad)
        playerInput.onControlsChanged += OnControlsChanged;
    }

    // Assigns control scheme to Player 1 depending on connected gamepads
    private void AssignControlScheme()
    {
        var gamepads = Gamepad.all;

        if (gamepads.Count >= 2)
        {
            // If two or more gamepads are connected, assign the second gamepad to Player 1
            Debug.Log("[INFO] Two gamepads detected. Assigning second gamepad to Player 1.");
            playerInput.SwitchCurrentControlScheme("Gamepad", gamepads[1]);
            usingGamepad = true;
        }
        else
        {
            // Otherwise, use keyboard & mouse for Player 1
            Debug.Log("[INFO] Using keyboard & mouse for Player 1.");
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            usingGamepad = false;
        }
    }

    // Called when an input device is added, removed, or changed
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added)
            {
                Debug.Log($"[DEVICE] Gamepad added. Re-evaluating input for Player 1.");
                // If Player 1 isn't using a gamepad but now there are at least two connected, assign second gamepad
                if (!usingGamepad && Gamepad.all.Count >= 2)
                {
                    AssignControlScheme();
                }
            }
            else if (change == InputDeviceChange.Removed)
            {
                Debug.Log($"[DEVICE] Gamepad removed. Re-evaluating input for Player 1.");
                // If Player 1 was using a gamepad but now less than two gamepads remain, fallback to keyboard & mouse
                if (usingGamepad && Gamepad.all.Count < 2)
                {
                    AssignControlScheme(); 
                }
            }
        }
    }

    // Called when PlayerInput detects a control scheme change (e.g., player switches from keyboard to gamepad mid-game)
    private void OnControlsChanged(PlayerInput input)
    {
        Debug.Log($"[CHANGE] Player 1 scheme changed to: {input.currentControlScheme}");
    }

    private void OnDestroy()
    {
        // Unsubscribe from device change events to prevent memory leaks
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}
