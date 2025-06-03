using UnityEngine;
using UnityEngine.UI;

public abstract class PowerUp : MonoBehaviour
{
    // Flag to check if the power-up has already been used
    public bool IsUsed;

    // Optional callback action to invoke when the power-up is used
    public System.Action OnUse { get; set; }

    [Header("UI")]
    // Icon texture to represent this power-up in the UI
    public Texture iconTexture;

    // Abstract method to define how the power-up activates for a given player
    // Must be implemented by subclasses
    public abstract void ActivatePowerUp(GameObject player);
}
