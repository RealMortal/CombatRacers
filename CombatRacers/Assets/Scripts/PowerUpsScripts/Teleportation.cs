using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Teleportation power-up inherits from PowerUp base class
public class Teleportation : PowerUp
{
    [SerializeField] private float radius = 30f; // Radius to search for other players to teleport with
    [SerializeField] private List<GameObject> teleportationEffectPrefabs; // VFX prefabs to play during teleport

    private List<GameObject> activeTeleportationEffects = new List<GameObject>(); // Track active VFX instances
    private GameObject activatingPlayer;           // The player who activated this power-up
    private GameObject playerWithMaxDistance;      // The player found farthest within radius to swap with

    // Called when this power-up is activated by a player GameObject
    public override void ActivatePowerUp(GameObject player)
    {
        // Try to get the PlayerPowerUpManager from player or parent
        PlayerPowerUpManager manager = player.GetComponentInParent<PlayerPowerUpManager>();
        if (manager != null)
        {
            Debug.Log("Teleport Power-Up Stored");

            // Use root GameObject in case player is child object
            activatingPlayer = player.transform.root.gameObject;

            // Store this power-up and provide callback to start teleportation effect later
            manager.StorePowerUp(this, StartTeleportation);
        }
    }

    // Finds the player farthest away within the radius to teleport with
    private GameObject PlayerToTeleport(GameObject player)
    {
        // Cast a sphere around the player to find potential targets
        RaycastHit[] hits = Physics.SphereCastAll(player.transform.position, radius, Vector3.up, 10f);

        float maxDistance = 0;
        GameObject farthestPlayer = null;

        foreach (RaycastHit hit in hits)
        {
            // Only consider other players, not self
            if (hit.transform.CompareTag("Player") && hit.transform.gameObject != player)
            {
                // Calculate distance to candidate player
                float distance = Vector3.Distance(hit.transform.position, player.transform.position);

                // Track player with maximum distance
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPlayer = hit.transform.gameObject;
                }

                // Draw debug line in editor for visualization
                Debug.DrawLine(player.transform.position, hit.transform.position, Color.green, 2f);
            }
        }

        return farthestPlayer;
    }

    // Called when the power-up effect actually starts (from PlayerPowerUpManager)
    public void StartTeleportation()
    {
        // Find the farthest player within radius to swap with
        playerWithMaxDistance = PlayerToTeleport(activatingPlayer);

        // Play teleportation effects on activating player
        PlayTeleportationEffects(activatingPlayer);

        // Play teleportation effects on target player, if found
        if (playerWithMaxDistance != null)
        {
            PlayTeleportationEffects(playerWithMaxDistance);
        }

        // Start coroutine to delay teleportation for effect timing
        StartCoroutine(TeleportWithDelay());
    }

    // Coroutine to wait for teleportation effects before swapping positions
    private IEnumerator TeleportWithDelay()
    {
        yield return new WaitForSeconds(1.5f);  // Wait for 1.5 seconds

        // Stop visual effects after delay
        StopTeleportationEffects();

        // Perform the actual teleportation position swap
        Teleport();
    }

    // Swaps positions of activating player and target player
    public void Teleport()
    {
        if (activatingPlayer == null || playerWithMaxDistance == null)
            return;

        // Store activating player's previous position
        Vector3 previousPosition = activatingPlayer.transform.position;

        // Teleport activating player to target player's position (slightly offset up)
        activatingPlayer.transform.position = playerWithMaxDistance.transform.position + (Vector3.up * 0.5f);

        // Teleport target player to activating player's old position (slightly offset up)
        playerWithMaxDistance.transform.position = previousPosition + (Vector3.up * 0.5f);

        // Mark power-up as used so it can't be reused
        IsUsed = true;

        // Clear references for safety
        activatingPlayer = null;
        playerWithMaxDistance = null;
    }

    // Instantiates and plays teleportation VFX on specified player
    private void PlayTeleportationEffects(GameObject playerToPlayEffect)
    {
        foreach (var effectPrefab in teleportationEffectPrefabs)
        {
            // Instantiate effect as child of player transform
            GameObject instance = Instantiate(effectPrefab, playerToPlayEffect.transform);
            activeTeleportationEffects.Add(instance);

            // Reset local position and rotation to center on player
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            // If effect has a ParticleSystem, schedule it for destruction after it finishes
            ParticleSystem ps = instance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(instance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }

    // Stops and destroys all active teleportation VFX instances
    private void StopTeleportationEffects()
    {
        foreach (var vfx in activeTeleportationEffects)
        {
            if (vfx != null)
                Destroy(vfx);
        }

        activeTeleportationEffects.Clear();
    }

    // Visualize radius sphere in editor when power-up is active
    private void OnDrawGizmos()
    {
        if (activatingPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(activatingPlayer.transform.position, radius);
        }
    }
}
