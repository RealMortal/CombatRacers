using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPowerUpManager : MonoBehaviour
{
    public PowerUp storedPowerUp;
    private System.Action storedEffect;

     private NewCarController controller;
    private PlayerInput playerInput;

    [SerializeField]private RawImage powerUpImage;
    [SerializeField] private Texture emptyPowerUpIcon;

    private void Start()
    {
        controller  = GetComponent<NewCarController>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["UsePowerUp"].performed += OnUsePowerUp;
    }
    public void SetPowerUpImage(RawImage image)
    {
        powerUpImage = image;
    }
    public void StorePowerUp(PowerUp powerUp, System.Action effect)
    {
        storedPowerUp = powerUp;
        storedEffect = effect;

        if (powerUp.iconTexture != null)
            powerUpImage.texture = powerUp.iconTexture;
    }

    private void OnUsePowerUp(InputAction.CallbackContext context)
    {
        if (storedPowerUp != null)
        {
            storedEffect?.Invoke();
            controller.SetHasActivePowerUp(false);

            powerUpImage.texture = emptyPowerUpIcon;

            storedPowerUp = null;
            storedEffect = null;
        }
    }
}
