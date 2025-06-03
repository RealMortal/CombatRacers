using UnityEngine;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    // Static list to keep track of all power-up game objects in the scene
    public static List<GameObject> list = new List<GameObject>();

    // List of available power-up prefabs or instances that this manager can assign
    public List<PowerUp> powerUps;

    // The power-up chosen randomly from the list to activate
    private PowerUp chosenPowerUp;

    private void Awake()
    {
        // Add this game object to the static list if not already present
        if (!list.Contains(gameObject))
            list.Add(gameObject);
    }

    private void Start()
    {
        // Randomly select a power-up from the list if any are available
        if (powerUps.Count > 0)
        {
            chosenPowerUp = powerUps[Random.Range(0, powerUps.Count)];
        }
    }

    // Called when another collider enters this trigger collider
    void OnTriggerEnter(Collider other)
    {
        // Only respond to collisions with objects tagged as "Player"
        if (!other.CompareTag("Player")) return;

        // Get the NewCarController component from the player's parent hierarchy
        var player = other.GetComponentInParent<NewCarController>();

        // If no player controller found, or no power-up chosen, or player already has an active power-up, do nothing
        if (player == null || chosenPowerUp == null || player.HasActivePowerUp()) return;

        // Mark player as having an active power-up
        player.SetHasActivePowerUp(true);

        // Activate the chosen power-up on the player
        chosenPowerUp.ActivatePowerUp(other.gameObject);

        // Deactivate this power-up object so it can't be collected again
        gameObject.SetActive(false);
    }
}
