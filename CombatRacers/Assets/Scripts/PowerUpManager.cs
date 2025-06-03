using UnityEngine;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    public static List<GameObject> list = new List<GameObject>();

    public List<PowerUp> powerUps;
    private PowerUp chosenPowerUp;

    private void Awake()
    {
        if (!list.Contains(gameObject))
            list.Add(gameObject);
    }

    private void Start()
    {
        if (powerUps.Count > 0)
        {
            chosenPowerUp = powerUps[Random.Range(0, powerUps.Count)];
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponentInParent<NewCarController>();
        if (player == null || chosenPowerUp == null || player.HasActivePowerUp()) return;

        player.SetHasActivePowerUp(true);
        chosenPowerUp.ActivatePowerUp(other.gameObject);
        gameObject.SetActive(false); 
    }
}
