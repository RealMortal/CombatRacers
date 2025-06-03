using UnityEngine;

public class PowerUpSpin : MonoBehaviour
{
    public float rotationSpeed = 90f; 
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rotate around global Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Calculate new Y position with sine wave float
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        // Keep X and Z fixed from start position to avoid drift
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
