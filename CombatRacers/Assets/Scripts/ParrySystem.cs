using UnityEngine;
using UnityEngine.InputSystem;

public class ParrySystem : MonoBehaviour
{
    private Rigidbody rb;                    // Rigidbody of the player/car
    private Vector3 preHitVelocity;          // Velocity before collision, used to nullify collision effects
    private PlayerInput playerInput;         // PlayerInput component to detect input actions

    [Header("Parry Timing")]
    [SerializeField] private float perfectParryWindow = 0.2f;   // Time window within parry duration considered a "perfect parry"
    [SerializeField] private float totalParryDuration = 0.4f;   // Total time player is in parry state
    [SerializeField] private float parryCooldownTime = 1.5f;    // Cooldown time after a parry ends before player can parry again

    [Header("Parry Effects")]
    [SerializeField] private float baseBoostForce = 500f;       // Base force applied as a boost when parry succeeds

    private float parryTimer;              // Counts down parry duration while parrying
    private float parryCooldown;           // Counts down cooldown time between parries
    [HideInInspector] public bool isParrying = false;  // Flag to indicate if currently parrying

    // Returns parry cooldown progress as a value between 0 and 1 (for UI, etc.)
    public float GetParryCooldownNormalized()
    {
        return Mathf.Clamp01(parryCooldown / parryCooldownTime);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // Subscribe to the "Parry" input action performed event
        playerInput.actions["Parry"].performed += OnParryPerformed;
    }

    private void OnDestroy()
    {
        // Unsubscribe from input event when this object is destroyed to avoid memory leaks
        if (playerInput != null)
            playerInput.actions["Parry"].performed -= OnParryPerformed;
    }

    // Called when the Parry input action is performed
    private void OnParryPerformed(InputAction.CallbackContext context)
    {
        // Only start parry if cooldown has elapsed
        if (parryCooldown <= 0f)
        {
            isParrying = true;                   // Enter parry state
            parryTimer = totalParryDuration;    // Reset parry timer
        }
    }

    void FixedUpdate()
    {
        // Store current velocity before collision to restore it later (nullify physics effects of collision)
        preHitVelocity = rb.linearVelocity;

        // Countdown the cooldown timer if parry is on cooldown
        if (parryCooldown > 0f)
            parryCooldown -= Time.fixedDeltaTime;
    }

    void Update()
    {
        if (isParrying)
        {
            // Decrease parry timer during parry state
            parryTimer -= Time.deltaTime;

            // End parry state when timer runs out, start cooldown
            if (parryTimer <= 0f)
            {
                isParrying = false;
                parryCooldown = parryCooldownTime;
            }
        }
    }

    // Called when this object collides with another collider
    private void OnCollisionEnter(Collision collision)
    {
        // Only process collisions if currently parrying
        if (!isParrying) return;

        // Check if collision occurred during the perfect parry window (early part of parry)
        bool isPerfectParry = parryTimer > (totalParryDuration - perfectParryWindow);

        if (isPerfectParry)
        {
            Debug.Log("Perfect Parry!");
            NullifyCollision();               // Cancel collision impact
            ApplyBoost(1.5f);                 // Apply stronger boost for perfect parry
        }
        else
        {
            Debug.Log("Normal Parry");
            NullifyCollision();               // Cancel collision impact
            ApplyBoost(1.0f);                 // Apply normal boost
        }

        // End parry state and start cooldown after collision
        isParrying = false;
        parryCooldown = parryCooldownTime;
    }

    // Nullify the collision effects by restoring previous velocity and zeroing angular velocity
    void NullifyCollision()
    {
        rb.linearVelocity = preHitVelocity;
        rb.angularVelocity = Vector3.zero;
    }

    // Apply a forward boost force multiplied by given multiplier
    void ApplyBoost(float multiplier)
    {
        float boostForce = baseBoostForce * multiplier;
        rb.AddForce(transform.forward * boostForce, ForceMode.Acceleration);
    }
}
