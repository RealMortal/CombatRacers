using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public Rigidbody target;

    public float maxSpeed = 200f; // Set this to your max speed (km/h)

    public float minSpeedArrowAngle = -90f;
    public float maxSpeedArrowAngle = 90f;

    [Header("UI")]
    public TextMeshProUGUI speedLabel;
    public RectTransform arrow;

    private float speed = 0.0f;

    public void SetTarget(Rigidbody rb)
    {
        target = rb;
    }

    private void Update()
    {
        if (target == null) return;

        // Calculate speed in km/h
        speed = target.velocity.magnitude * 3.6f;

        // Clamp speed to maxSpeed for arrow rotation
        float clampedSpeed = Mathf.Clamp(speed, 0, maxSpeed);

        // Update speed text
        if (speedLabel != null)
            speedLabel.text = ((int)speed).ToString() + " km/h";

        // Rotate the arrow between min and max angle based on clamped speed ratio
        if (arrow != null)
        {
            float angle = Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, clampedSpeed / maxSpeed);
            arrow.localEulerAngles = new Vector3(0, 0, angle);
        }
    }
}
