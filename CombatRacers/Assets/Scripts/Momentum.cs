using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Momentum : MonoBehaviour
{
    public float rayDistance = 20f;           // Distance for the forward raycast to detect players ahead
    public float boostMultiplier = 1.2f;      // How much to multiply the acceleration when boosting
    public float boostDuration = 1f;          // How long the boost lasts in seconds

    private Rigidbody rb;                      // Rigidbody component of this car
    private NewCarController controller;      // Reference to the car controller to adjust acceleration
    private float originalAcceleration;       // Store original acceleration value to reset later
    private Coroutine activeBoost;             // Reference to currently running boost coroutine (if any)

    void Start()
    {
        rb = GetComponent<Rigidbody>();       // Cache Rigidbody component
    }

    // Set the car controller and store its original acceleration value for boosting
    public void SetPowerUp(NewCarController controller)
    {
        this.controller = controller;
        originalAcceleration = controller.GetAcceleration();
    }

    void FixedUpdate()
    {
        // Raycast from slightly above car's position, forward, to detect players in front
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, rayDistance))
        {
            // Check if the hit object is tagged "Player"
            if (hit.collider.CompareTag("Player"))
            {
                // Get mass of the player ahead
                float mass = hit.transform.gameObject.GetComponent<Rigidbody>().mass;

                // Calculate proximity factor if the other player's mass is greater than this car's mass
                float proximity = (mass > rb.mass) ? (rayDistance - hit.distance) / rayDistance : 0;

                // Only trigger boost if proximity is greater than 0.3 (close enough)
                if (proximity > 0.3f)
                {
                    TriggerBoost();
                }
            }
        }
    }

    // Starts the boost effect, cancelling any existing one
    void TriggerBoost()
    {
        if (activeBoost != null)
        {
            StopCoroutine(activeBoost);            // Stop currently running boost coroutine
            controller.SetAcceleration(originalAcceleration); // Reset acceleration to original
        }

        activeBoost = StartCoroutine(ApplyBoost());  // Start new boost coroutine
    }

    // Coroutine to apply boost for a duration, then reset acceleration
    IEnumerator ApplyBoost()
    {
        controller.SetAcceleration(originalAcceleration * boostMultiplier);  // Boost acceleration

        yield return new WaitForSeconds(boostDuration);  // Wait for boost duration

        controller.SetAcceleration(originalAcceleration);  // Reset acceleration to normal

        activeBoost = null;  // Clear the active boost coroutine reference
    }
}
