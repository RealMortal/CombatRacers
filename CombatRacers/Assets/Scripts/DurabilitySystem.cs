using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the durability (health) of the car, applying damage from collisions,
/// triggering destruction effects, and handling respawning.
/// </summary>
public class DurabilitySystem : MonoBehaviour
{
    [SerializeField] private float maxDurability = 100f;       // Maximum durability value
    [SerializeField] private float damageMultiplyer = 0.01f;    // Multiplier to convert impact force into damage

    private ParrySystem parrySystem;    // Reference to parry system to check if currently parrying
    private Rigidbody rb;               // Rigidbody of the car for physics control
    private bool isParrying;            // Flag to know if currently parrying
    ParticleSystem explosion;           // Explosion particle effect prefab to instantiate on destruction
    private float currentDurability;    // Current durability value
    PlayerInput playerInput;            // Player input reference for manual destruction

    /// <summary>
    /// Assigns the explosion particle system prefab to be used on destruction.
    /// </summary>
    public void SetUpParticleSystem(ParticleSystem explosion)
    {
        this.explosion = explosion;
    }

    /// <summary>
    /// Returns the durability value normalized between 0 and 1.
    /// Useful for UI bars.
    /// </summary>
    public float GetDurabilityNormalized()
    {
        return Mathf.Clamp01(currentDurability / maxDurability);
    }

    private void Start()
    {
        // Get components needed for durability and input management
        rb = GetComponent<Rigidbody>();
        parrySystem = GetComponent<ParrySystem>();
        playerInput = GetComponent<PlayerInput>();
        currentDurability = maxDurability; // Start fully durable
    }

    private void Update()
    {
        // Check if the player manually triggers destruction (for testing/debug)
        if (playerInput.actions["Destroy"].WasPressedThisFrame())
        {
            OnCarDestroyed();
        }

        // Update the parrying status from ParrySystem
        isParrying = parrySystem.isParrying;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only apply damage if the collision is with Player or Obstacle and not parrying
        bool isTargetRelevant = collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Obstacle");
        if (isParrying || !isTargetRelevant)
        {
            return;
        }

        GameObject other = collision.gameObject;
        NewCarController otherController = other.GetComponentInParent<NewCarController>();

        // If the other car can one-shot destroy, instantly destroy this car
        if (otherController != null && otherController.isAbleToOneShot)
        {
            Debug.Log("Destroyed instantly");
            OnCarDestroyed();
            return;
        }

        // Calculate damage from impact force and apply it
        float impactForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        float damage = impactForce * damageMultiplyer;

        Debug.Log($"Applying {damage:F1} damage from impact force.");
        ApplyDamage(damage);
    }

    /// <summary>
    /// Reduces durability by damage amount and handles destruction if durability hits zero.
    /// </summary>
    void ApplyDamage(float damage)
    {
        currentDurability -= damage;
        currentDurability = Mathf.Clamp(currentDurability, 0, maxDurability);

        Debug.Log($"Took {damage:F1} damage! Remaining: {currentDurability}");

        if (currentDurability <= 0)
        {
            OnCarDestroyed();
        }
    }

    /// <summary>
    /// Handles what happens when the car is destroyed:
    /// plays explosion effect, disables the car, then schedules a respawn.
    /// </summary>
    void OnCarDestroyed()
    {
        // Instantiate and play explosion particle effect slightly above car position
        if (explosion != null)
        {
            ParticleSystem explosion_ = Instantiate(
                explosion,
                new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z),
                Quaternion.identity
            );
            explosion_.Play();

            Destroy(explosion_.gameObject, explosion_.main.duration + explosion_.main.startLifetime.constantMax);
        }

        // Reset rotation and deactivate the car object
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);

        // Respawn after 3 seconds
        Invoke(nameof(Respawn), 3f);
    }

    /// <summary>
    /// Respawns the car by resetting durability, velocity, and enabling the object.
    /// </summary>
    void Respawn()
    {
        currentDurability = maxDurability;

        // Reset velocity to stop any ongoing motion
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        gameObject.SetActive(true);
    }
}
