using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedBoost : PowerUp
{
    public float speedMultiplier = 2.0f;
    public float duration = 2.0f;

    private float originalSpeed;
    private Coroutine activeBoost;

    private NewCarController controller;
    private CinemachineCamera playerCamera;

    public override void ActivatePowerUp(GameObject player)
    {
       
        PlayerPowerUpManager manager = player.GetComponentInParent<PlayerPowerUpManager>();
        if (manager != null)
        {
            player = player.transform.root.gameObject;
            Debug.Log("Speed Boost Stored!");

            controller = player.GetComponent<NewCarController>();
            playerCamera = controller.GetPlayerCamera();

            if (controller == null)
            {
                Debug.LogWarning("SpeedBoost: Missing controller ");
                return;
            }
            if(playerCamera == null)
            {
                Debug.LogWarning("Missing camera");
            }

            originalSpeed = controller.GetAcceleration();

            // Store this boost effect in the manager
            manager.StorePowerUp(this, () => ApplySpeedBoost(speedMultiplier, duration));
        }
    }

    public void ApplySpeedBoost(float boostMultiplier, float boostDuration)
    {
        if (activeBoost != null)
        {
            StopCoroutine(activeBoost);
            controller.SetAcceleration(originalSpeed); // Reset speed before applying new boost
        }

        // Start a new boost
        activeBoost = StartCoroutine(ApplyBoost(boostMultiplier, boostDuration));
    }

    private IEnumerator ApplyBoost(float boostMultiplier, float boostDuration)
    {
        Debug.Log("Speed Boost Activated!");
        controller.SetAcceleration(boostMultiplier * originalSpeed);
        yield return StartCoroutine(ChangeFOV(70, 100, 0.3f));

        yield return new WaitForSeconds(boostDuration);

        controller.SetAcceleration(originalSpeed);
        yield return StartCoroutine(ChangeFOV(100, 70, 0.3f));

        IsUsed = true;
        activeBoost = null;
    }

    private IEnumerator ChangeFOV(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            playerCamera.Lens.FieldOfView = Mathf.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.Lens.FieldOfView = to;
    }
}
