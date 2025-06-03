using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : PowerUp
{
    [SerializeField] private float radius = 30f;
    [SerializeField] private List<GameObject> teleportationEffectPrefabs;

    private List<GameObject> activeTeleportationEffects = new List<GameObject>();
    private GameObject activatingPlayer;
    private GameObject playerWithMaxDistance;

    public override void ActivatePowerUp(GameObject player)
    {
        PlayerPowerUpManager manager = player.GetComponentInParent<PlayerPowerUpManager>();
        if (manager != null)
        {
            Debug.Log("Teleport Power-Up Stored");
            activatingPlayer = player.transform.root.gameObject; 
            manager.StorePowerUp(this, StartTeleportation);
        }
    }

    private GameObject PlayerToTeleport(GameObject player)
    {
        RaycastHit[] hits = Physics.SphereCastAll(player.transform.position, radius, Vector3.up, 10f);

        float maxDistance = 0;
        GameObject farthestPlayer = null;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.CompareTag("Player") && hit.transform.gameObject != player)
            {
                float distance = Vector3.Distance(hit.transform.position, player.transform.position);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPlayer = hit.transform.gameObject;
                }

                Debug.DrawLine(player.transform.position, hit.transform.position, Color.green, 2f);
            }
        }

        return farthestPlayer;
    }

    public void StartTeleportation()
    {
        playerWithMaxDistance = PlayerToTeleport(activatingPlayer);

        PlayTeleportationEffects(activatingPlayer);
        if (playerWithMaxDistance != null)
        {
            PlayTeleportationEffects(playerWithMaxDistance);
        }

        StartCoroutine(TeleportWithDelay());
    }

    private IEnumerator TeleportWithDelay()
    {
        yield return new WaitForSeconds(1.5f);

        StopTeleportationEffects();
        Teleport();
    }

    public void Teleport()
    {
        if (activatingPlayer == null || playerWithMaxDistance == null)
            return;

        Vector3 previousPosition = activatingPlayer.transform.position;
        activatingPlayer.transform.position = playerWithMaxDistance.transform.position + (Vector3.up * 0.5f);
        playerWithMaxDistance.transform.position = previousPosition + (Vector3.up * 0.5f);

        IsUsed = true;

        activatingPlayer = null;
        playerWithMaxDistance = null;
    }

    private void PlayTeleportationEffects(GameObject playerToPlayEffect)
    {
        foreach (var effectPrefab in teleportationEffectPrefabs)
        {
            GameObject instance = Instantiate(effectPrefab, playerToPlayEffect.transform);
            activeTeleportationEffects.Add(instance);

            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            ParticleSystem ps = instance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(instance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }

    private void StopTeleportationEffects()
    {
        foreach (var vfx in activeTeleportationEffects)
        {
            if (vfx != null)
                Destroy(vfx);
        }

        activeTeleportationEffects.Clear();
    }

    private void OnDrawGizmos()
    {
        if (activatingPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(activatingPlayer.transform.position, radius);
        }
    }
}
