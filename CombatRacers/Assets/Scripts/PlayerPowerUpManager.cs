using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPowerUpManager : MonoBehaviour
{
    // Currently stored power-up instance
    public PowerUp storedPowerUp;

    // The effect/action associated with the stored power-up
    private System.Action storedEffect;

    private NewCarController controller;   // Reference to the car controller script
    private PlayerInput playerInput;       // Reference to the player input component

    [SerializeField] private RawImage powerUpImage;       // UI element to show power-up icon
    [SerializeField] private Texture emptyPowerUpIcon;    // Default icon when no power-up is stored

    private void Start()
    {
        // Cache references to required components
        controller = GetComponent<NewCarController>();
        playerInput = GetComponent<PlayerInput>();

        // Subscribe to the "UsePowerUp" input action to handle power-up activation
        playerInput.actions["UsePowerUp"].performed += OnUsePowerUp;
    }

    // Allows setting the UI image element externally (useful if assigned later)
    public void SetPowerUpImage(RawImage image)
    {
        powerUpImage = image;
    }

    // Store a new power-up and its effect, update the UI icon accordingly
    public void StorePowerUp(PowerUp powerUp, System.Action effect)
    {
        storedPowerUp = powerUp;
        storedEffect = effect;

        // Update UI icon to show the stored power-up's texture, if available
        if (powerUp.iconTexture != null)
            powerUpImage.texture = powerUp.iconTexture;
    }

    // Called when player triggers the "UsePowerUp" input action
    private void OnUsePowerUp(InputAction.CallbackContext context)
    {
        // Only activate if a power-up is currently stored
        if (storedPowerUp != null)
        {
            // Invoke the stored power-up effect (if any)
            storedEffect?.Invoke();

            // Notify the controller that the player no longer has an active power-up
            controller.SetHasActivePowerUp(false);

            // Reset the power-up icon to the empty/default texture
            powerUpImage.texture = emptyPowerUpIcon;

            // Clear the stored power-up and effect references
            storedPowerUp = null;
            storedEffect = null;
        }
    }
}
