using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    [Header("Wheel Settings")]
    public Transform[] wheels; 
    public float wheelRadius = 0.5f; 
    public float maxSpeed = 100f;

    private Rigidbody carRigidbody;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        RotateWheels();
    }

    private void RotateWheels()
    {
       
        float carSpeed = carRigidbody.linearVelocity.magnitude;


        float wheelRotationSpeed = (carSpeed / (2 * Mathf.PI * wheelRadius)) * 360f; 

        
        foreach (Transform wheel in wheels)
        {
  
            wheel.Rotate(wheelRotationSpeed * Time.deltaTime, 0f, 0f);
        }
    }
}
