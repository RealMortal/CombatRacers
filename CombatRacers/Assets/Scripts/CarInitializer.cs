using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Initializes various components and UI elements of the player's car at runtime.
/// Links UI elements, camera, checkpoints, and particle effects to the appropriate scripts on the car.
/// </summary>
public class CarInitializer : MonoBehaviour
{
    /// <summary>
    /// Sets up references between this car's components and UI/game systems.
    /// Uses null-conditional checks to avoid errors if any component is missing.
    /// </summary>
    /// <param name="image">UI element for displaying power-up image</param>
    /// <param name="controller">Reference to the car controller (used for power-up linking)</param>
    /// <param name="progressText">Text element showing lap progress</param>
    /// <param name="LapText">Text element showing current lap number</param>
    /// <param name="checkPoints">Array of checkpoints for lap tracking</param>
    /// <param name="parryCD">UI image representing cooldown for parry</param>
    /// <param name="HealthBar">UI image representing car health</param>
    /// <param name="cam">Cinemachine camera following the car</param>
    /// <param name="explosion">Particle system for durability/explosion effects</param>
    public void Initialize(
        RawImage image,
        NewCarController controller,
        TextMeshProUGUI progressText,
        TextMeshProUGUI LapText,
        CheckPoint[] checkPoints,
        Image parryCD,
        Image HealthBar,
        CinemachineCamera cam,
        ParticleSystem explosion)
    {
        // Link power-up manager to power-up UI image
        GetComponent<PlayerPowerUpManager>()?.SetPowerUpImage(image);

        // Setup lap manager UI and checkpoints
        GetComponent<LapManager>()?.SetGUI(progressText, LapText);
        GetComponent<LapManager>()?.SetCheckPointsList(checkPoints);

        // Assign player camera to car controller
        GetComponent<NewCarController>()?.SetPlayerCamera(cam);

        // Setup particle system for durability/explosion effects
        GetComponent<DurabilitySystem>()?.SetUpParticleSystem(explosion);

        // Link momentum/power-up logic to car controller
        GetComponent<Momentum>()?.SetPowerUp(controller);

        // Setup UI controller with parry cooldown and health bar UI elements
        GetComponent<PlayerUIController>()?.SetGUI(parryCD, HealthBar);
    }
}
