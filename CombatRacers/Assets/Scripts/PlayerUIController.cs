using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private Image powerUpIcon;
    [SerializeField] private Image parryCooldownBar;
    [SerializeField] private Image healthBar;
    [SerializeField] private List<RawImage> imageList;

    private ParrySystem parrySystem;
    private DurabilitySystem health;
    private PlayerPowerUpManager PUM;
    private PowerUp storedPowerUp;
    public void SetGUI(Image parryBar, Image health)
    {
        parryCooldownBar = parryBar;
        healthBar = health;
    }

    private void Start()
    {
        PUM = GetComponent<PlayerPowerUpManager>();
        parrySystem = GetComponent<ParrySystem>();
        health = GetComponent<DurabilitySystem>();
    }

    void Update()
    {
        if (parrySystem != null && parryCooldownBar != null)
            parryCooldownBar.fillAmount = parrySystem.GetParryCooldownNormalized();

        if (health != null && healthBar != null)
            healthBar.fillAmount = health.GetDurabilityNormalized();
    }
}

