using UnityEngine;

/// <summary>
/// Represents a checkpoint in the race track.
/// When a player passes through this checkpoint, it notifies the player's LapManager.
/// </summary>
public class CheckPoint : MonoBehaviour
{
    // Unique identifier for this checkpoint (e.g., order in the lap)
    public int checkpointID;

    /// <summary>
    /// Called when another collider enters this checkpoint's trigger collider.
    /// If the collider belongs to the player, notify the player's LapManager that this checkpoint was passed.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player (tagged "Player")
        if (other.CompareTag("Player"))
        {
            // Find the LapManager component in the parent hierarchy and notify it
            other.GetComponentInParent<LapManager>()?.CheckpointPassed(checkpointID);
        }
    }
}
