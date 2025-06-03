using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// SpeedBoost inherits from abstract PowerUp class
public class SpeedBoost : PowerUp
{
    // Multiplier to increase player's speed during boost
    public float speedMultiplier = 2.0f;
    // Duration for how long the boost lasts
    public float duration = 2.0f;

    // Original acceleration value to restore after boost ends
    private float originalSpeed;
    // Reference to currently running boost coroutine
    private Coroutine activeBoost;

    private NewCarController controller;      // Reference to car controller
    private CinemachineCamera playerCamera;  // Reference to player's camera (for FOV effects)

    // Override the abstract method to activate this power-up on a player GameObject
    public override void ActivatePowerUp(GameObject player)
    {
        // Get the PlayerPowerUpManager from the player or its parent
        PlayerPowerUpManager manager = player.GetComponentInParent<PlayerPowerUpManager>();
        if (manager != null)
        {
            // Use root GameObject in case player is a child object
            player = player.transform.root.gameObject;
            Debug.Log("Speed Boost Stored!");

            // Get car controller and camera references from player
            controller = player.GetComponent<NewCarController>();
            playerCamera = controller.GetPlayerCamera();

            // Safety checks
            if (controller == null)
            {
                Debug.LogWarning("SpeedBoost: Missing controller ");
                return;
            }
            if (playerCamera == null)
            {
                Debug.LogWarning("Missing camera");
            }

            // Save original acceleration to restore later
            originalSpeed = controller.GetAcceleration();

            // Store this power-up and provide an action callback to apply the boost later
            manager.StorePowerUp(this, () => ApplySpeedBoost(speedMultiplier, duration));
        }
    }

    // Applies the speed boost effect with given multiplier and duration
    public void ApplySpeedBoost(float boostMultiplier, float boostDuration)
    {
        // If there is an active boost, stop it and reset speed before starting a new one
        if (activeBoost != null)
        {
            StopCoroutine(activeBoost);
            controller.SetAcceleration(originalSpeed); // Reset speed
        }

        // Start the coroutine to handle boost effect and timing
        activeBoost = StartCoroutine(ApplyBoost(boostMultiplier, boostDuration));
    }

    // Coroutine that manages the boost effect duration and camera FOV change
    private IEnumerator ApplyBoost(float boostMultiplier, float boostDuration)
    {
        Debug.Log("Speed Boost Activated!");

        // Increase player acceleration
        controller.SetAcceleration(boostMultiplier * originalSpeed);
        // Smoothly change camera FOV to give sense of speed increase (from 70 to 100)
        yield return StartCoroutine(ChangeFOV(70, 100, 0.3f));

        // Wait for the boost duration while effect is active
        yield return new WaitForSeconds(boostDuration);

        // Restore original acceleration after boost ends
        controller.SetAcceleration(originalSpeed);
        // Smoothly revert camera FOV back to normal (from 100 to 70)
        yield return StartCoroutine(ChangeFOV(100, 70, 0.3f));

        // Mark this power-up as used so it can't be reused
        IsUsed = true;
        // Clear the reference to active boost coroutine
        activeBoost = null;
    }

    // Coroutine for smooth camera Field of View (FOV) transition effect
    private IEnumerator ChangeFOV(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Calculate interpolation factor (0 to 1)
            float t = elapsed / duration;
            // Interpolate camera FOV between from and to values
            playerCamera.Lens.FieldOfView = Mathf.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null; // wait for next frame
        }

        // Ensure final FOV is set exactly
        playerCamera.Lens.FieldOfView = to;
    }
}
