using UnityEngine;
using UnityEngine.InputSystem;

public class ParrySystem : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 preHitVelocity;
    private PlayerInput playerInput;

    [Header("Parry Timing")]
    [SerializeField] private float perfectParryWindow = 0.2f;
    [SerializeField] private float totalParryDuration = 0.4f;
    [SerializeField] private float parryCooldownTime = 1.5f;

    [Header("Parry Effects")]
    [SerializeField] private float baseBoostForce = 500f;

    private float parryTimer;
    private float parryCooldown;
    [HideInInspector] public bool isParrying = false;

    public float GetParryCooldownNormalized()
    {
        return Mathf.Clamp01(parryCooldown / parryCooldownTime);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["Parry"].performed += OnParryPerformed;
    }

    private void OnDestroy()
    {
        if (playerInput != null)
            playerInput.actions["Parry"].performed -= OnParryPerformed;
    }

    private void OnParryPerformed(InputAction.CallbackContext context)
    {
        if (parryCooldown <= 0f)
        {
            isParrying = true;
            parryTimer = totalParryDuration;
        }
    }

    void FixedUpdate()
    {
        preHitVelocity = rb.linearVelocity;

        if (parryCooldown > 0f)
            parryCooldown -= Time.fixedDeltaTime;
    }

    void Update()
    {
        if (isParrying)
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0f)
            {
                isParrying = false;
                parryCooldown = parryCooldownTime;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isParrying) return;

        bool isPerfectParry = parryTimer > (totalParryDuration - perfectParryWindow);

        if (isPerfectParry)
        {
            Debug.Log("Perfect Parry!");
            NullifyCollision();
            ApplyBoost(1.5f);
        }
        else
        {
            Debug.Log("Normal Parry");
            NullifyCollision();
            ApplyBoost(1.0f);
        }

        isParrying = false;
        parryCooldown = parryCooldownTime;
    }

    void NullifyCollision()
    {
        rb.linearVelocity = preHitVelocity;
        rb.angularVelocity = Vector3.zero;
    }

    void ApplyBoost(float multiplier)
    {
        float boostForce = baseBoostForce * multiplier;
        rb.AddForce(transform.forward * boostForce, ForceMode.Acceleration);
    }
}
