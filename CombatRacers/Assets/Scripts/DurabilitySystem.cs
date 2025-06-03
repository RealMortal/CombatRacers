using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DurabilitySystem : MonoBehaviour
{
    [SerializeField] private float maxDurability = 100f;
    [SerializeField] private float damageMultiplyer = 0.01f;
  
    private ParrySystem parrySystem;
    private Rigidbody rb;
    private bool isParrying;
    ParticleSystem explosion;
    private float currentDurability;
    PlayerInput playerInput;


    public void SetUpParticleSystem(ParticleSystem explosion)
    {
        this.explosion = explosion;
    }

  
    public float GetDurabilityNormalized()
    {
        return Mathf.Clamp01(currentDurability / maxDurability);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        parrySystem = GetComponent<ParrySystem>();
        playerInput = GetComponent<PlayerInput>();
        currentDurability = maxDurability;
    }

    private void Update()
    {
        if (playerInput.actions["Destroy"].WasPressedThisFrame())
        {
            OnCarDestroyed();
        }
            isParrying = parrySystem.isParrying;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool isTargetRelevant = collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Obstacle");
        if (isParrying || !isTargetRelevant)
        {
            return;
        }

        GameObject other = collision.gameObject;

        NewCarController otherController = other.GetComponentInParent<NewCarController>();

        if (otherController != null && otherController.isAbleToOneShot)
        {
            Debug.Log("Destroyed instantly");
            OnCarDestroyed();
            return;
        }


        float impactForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        float damage = impactForce * damageMultiplyer;

        Debug.Log($"Applying {damage:F1} damage from impact force.");
        ApplyDamage(damage);
    }



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

    void OnCarDestroyed()
    {
        if (explosion != null)
        {
            ParticleSystem explosion_ = Instantiate(explosion, new Vector3(transform.position.x,transform.position.y+1.5f,transform.position.z), Quaternion.identity);
            explosion_.Play();

            Destroy(explosion_.gameObject, explosion_.main.duration + explosion_.main.startLifetime.constantMax);
        }
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), 3f);

    }

    void Respawn()
    {
        currentDurability = maxDurability;


        rb.linearVelocity = Vector3.zero;

        gameObject.SetActive(true);
    }

}
