using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CarInitializer : MonoBehaviour
{
    public void Initialize(RawImage image,NewCarController controller,TextMeshProUGUI progressText, TextMeshProUGUI LapText, CheckPoint[] checkPoints,Image parryCD,Image HealthBar, CinemachineCamera cam,ParticleSystem explosion)
    {
        GetComponent<PlayerPowerUpManager>()?.SetPowerUpImage(image);
        GetComponent<LapManager>()?.SetGUI(progressText, LapText);
        GetComponent<LapManager>()?.SetCheckPointsList(checkPoints);
        GetComponent<NewCarController>()?.SetPlayerCamera(cam);
        GetComponent<DurabilitySystem>()?.SetUpParticleSystem(explosion);
        GetComponent<Momentum>()?.SetPowerUp(controller);
        GetComponent<PlayerUIController>()?.SetGUI(parryCD, HealthBar);
    }
}
