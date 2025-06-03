using NUnit.Framework; // (Note: NUnit not used in this script, can be removed)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    // UI Image showing the current power-up icon (not used in the script but declared)
    [SerializeField] private Image powerUpIcon;

    // UI Image acting as a fill bar to show parry cooldown progress
    [SerializeField] private Image parryCooldownBar;

    // UI Image acting as a fill bar to show player's health/durability
    [SerializeField] private Image healthBar;

    // List of RawImages (not used in current code but possibly for UI elements)
    [SerializeField] private List<RawImage> imageList;

    // Reference to the player’s parry system (to get cooldown info)
    private ParrySystem parrySystem;

    // Reference to player’s health/durability system
    private DurabilitySystem health;

    // Reference to the player's power-up manager
    private PlayerPowerUpManager PUM;

    // Currently stored power-up (not used in current code)
    private PowerUp storedPowerUp;

    // Method to assign UI elements externally for parry cooldown and health bars
    public void SetGUI(Image parryBar, Image health)
    {
        parryCooldownBar = parryBar;
        healthBar = health;
    }

    // Cache references to required components on start
    private void Start()
    {
        PUM = GetComponent<PlayerPowerUpManager>();
        parrySystem = GetComponent<ParrySystem>();
        health = GetComponent<DurabilitySystem>();
    }

    // Update UI elements every frame
    void Update()
    {
        // Update parry cooldown bar fill amount if references are valid
        if (parrySystem != null && parryCooldownBar != null)
            parryCooldownBar.fillAmount = parrySystem.GetParryCooldownNormalized();

        // Update health bar fill amount if references are valid
        if (health != null && healthBar != null)
            healthBar.fillAmount = health.GetDurabilityNormalized();
    }
}
