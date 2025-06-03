using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotOneKill : PowerUp
{
    // Flag to indicate if the player can one-shot kill others
    public bool isAbleToOneShot;

    // Reference to the player who activated this power-up
    private GameObject activatingPlayer;

    // Called when this power-up is activated by a player
    public override void ActivatePowerUp(GameObject player)
    {
        // Get the PlayerPowerUpManager component from the player's root object
        PlayerPowerUpManager manager = player.GetComponentInParent<PlayerPowerUpManager>();
        if (manager != null)
        {
            Debug.Log("Teleport Power-Up Stored");
            // Store the reference to the activating player
            activatingPlayer = player.transform.root.gameObject;

            // Store this power-up in the manager and assign the callback to start the one-shot effect
            manager.StorePowerUp(this, StartOneShot);
        }
    }

    // Method to start the one-shot effect coroutine
    private void StartOneShot()
    {
        StartCoroutine(StartTimer());
    }

    // Coroutine that enables the one-shot ability for a limited time, then disables it
    private IEnumerator StartTimer()
    {
        // Get the car controller of the activating player
        var controller = activatingPlayer.GetComponent<NewCarController>();

        if (controller != null)
        {
            controller.isAbleToOneShot = true;  // Enable one-shot ability
            yield return new WaitForSeconds(1.5f);  // Wait for 1.5 seconds duration
            controller.isAbleToOneShot = false;  // Disable one-shot ability
        }
    }

    // When this power-up is disabled, ensure the one-shot ability is turned off
    private void OnDisable()
    {
        isAbleToOneShot = false;
    }
}
