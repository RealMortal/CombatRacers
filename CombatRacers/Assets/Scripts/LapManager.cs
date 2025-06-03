using System.Linq;
using TMPro;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    public int totalCheckpoints = 5;           // Total number of checkpoints in the lap
    public int currentCheckpointIndex = 0;     // Index of the next checkpoint player must pass
    public int lapCount = 0;                    // Number of laps completed so far
    public TextMeshProUGUI progressText;       // UI text to show race progress percentage
    public TextMeshProUGUI lapText;            // UI text to show current lap count

    // Race progress as a normalized float from 0 to 1, including partial progress to next checkpoint
    public float raceProgress => (currentCheckpointIndex + partialProgressToNext) / totalCheckpoints;

    private float partialProgressToNext = 0f;  // Partial progress between current checkpoint and next checkpoint (0 to 1)

    [SerializeField]
    private CheckPoint[] checkpointPositions;  // Array of checkpoint references, sorted by checkpointID

    // Called every frame to update partial progress and update UI text
    void Update()
    {
        UpdatePartialProgress();
        // Update the progress text as a percentage integer (e.g. "45%")
        progressText.text = ((int)(raceProgress * 100f)).ToString() + "%";
    }

    // Assign UI text references for progress and laps
    public void SetGUI(TextMeshProUGUI progress, TextMeshProUGUI lap)
    {
        progressText = progress;
        lapText = lap;
    }

    // Set the list of checkpoints, ordered by their checkpointID
    public void SetCheckPointsList(CheckPoint[] list)
    {
        checkpointPositions = list.OrderBy(c => c.checkpointID).ToArray();
        totalCheckpoints = checkpointPositions.Length;
    }

    // Calculates partial progress between current checkpoint and the next checkpoint, based on player position
    private void UpdatePartialProgress()
    {
        // Only update if not at the last checkpoint yet
        if (currentCheckpointIndex < checkpointPositions.Length - 1)
        {
            Vector3 currentPos = checkpointPositions[currentCheckpointIndex].transform.position;
            Vector3 nextPos = checkpointPositions[currentCheckpointIndex + 1].transform.position;
            Vector3 playerPos = transform.position;

            float totalDistance = Vector3.Distance(currentPos, nextPos);  // Distance between current and next checkpoint
            float playerDistance = Vector3.Distance(playerPos, nextPos);  // Distance from player to next checkpoint

            // Partial progress is how close the player is to the next checkpoint, normalized [0,1]
            partialProgressToNext = Mathf.Clamp01(1f - (playerDistance / totalDistance));
        }
        else
        {
            // No partial progress if at last checkpoint
            partialProgressToNext = 0f;
        }
    }

    // Called when the player passes a checkpoint
    public void CheckpointPassed(int checkpointID)
    {
        // Only proceed if the checkpoint passed is the expected current checkpoint
        if (checkpointID == currentCheckpointIndex)
        {
            currentCheckpointIndex++;

            // If all checkpoints in the lap are passed, increment lap count and reset checkpoint index
            if (currentCheckpointIndex >= totalCheckpoints)
            {
                // Reactivate all power-ups from a static list (assumed managed elsewhere)
                foreach(GameObject powerUp in PowerUpManager.list)
                {
                    if (powerUp != null)
                    {
                        powerUp.SetActive(true);
                    }
                }
                
                currentCheckpointIndex = 0;  // Reset checkpoint index for new lap
                lapCount++;                  // Increment lap counter
                lapText.text = lapCount.ToString();  // Update lap UI text
                
                Debug.Log("Lap Completed! Total Laps: " + lapCount);
            }
        }
        else
        {
            Debug.Log("Wrong checkpoint!"); // Player hit checkpoints out of order
        }
    }
}
