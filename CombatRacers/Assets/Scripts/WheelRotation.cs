using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    [Header("Wheel Settings")]
    public Transform[] wheels; 
    public float wheelRadius = 0.5f; 
    public float maxSpeed = 100f; // in m/s or km/h? Adjust accordingly.

    private Rigidbody carRigidbody;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        RotateWheels();
    }

    private void RotateWheels()
    {
        if (carRigidbody == null) return;

        // Current speed in m/s
        float carSpeed = carRigidbody.velocity.magnitude;

        // Optional: Clamp speed if desired
        carSpeed = Mathf.Min(carSpeed, maxSpeed);

        // Calculate rotation speed in degrees per second
        // circumference = 2 * PI * radius
        // rotations per second = speed / circumference
        // degrees per second = rotations per second * 360
        float wheelRotationSpeed = (carSpeed / (2 * Mathf.PI * wheelRadius)) * 360f;

        // Rotate each wheel around its local X axis
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(wheelRotationSpeed * Time.fixedDeltaTime, 0f, 0f, Space.Self);
        }
    }
}
