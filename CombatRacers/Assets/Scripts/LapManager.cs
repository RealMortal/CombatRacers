using System.Linq;
using TMPro;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    public int totalCheckpoints = 5;
    public int currentCheckpointIndex = 0;
    public int lapCount = 0;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI lapText;

    public float raceProgress => (currentCheckpointIndex + partialProgressToNext) / totalCheckpoints;

    private float partialProgressToNext = 0f; 

    [SerializeField]
    private CheckPoint[] checkpointPositions;

   
    void Update()
    {
        UpdatePartialProgress();
        progressText.text = ((int)(raceProgress * 100f)).ToString() + "%";
    }
    public void SetGUI(TextMeshProUGUI progress, TextMeshProUGUI lap)
    {
        progressText = progress;
        lapText = lap;
    }

    public void SetCheckPointsList(CheckPoint[] list)
    {
        checkpointPositions = list.OrderBy(c => c.checkpointID).ToArray();
        totalCheckpoints = checkpointPositions.Length;
    }


    private void UpdatePartialProgress()
    {
        if (currentCheckpointIndex < checkpointPositions.Length - 1)
        {
            Vector3 currentPos = checkpointPositions[currentCheckpointIndex].transform.position;
            Vector3 nextPos = checkpointPositions[currentCheckpointIndex + 1].transform.position;
            Vector3 playerPos = transform.position;

            float totalDistance = Vector3.Distance(currentPos, nextPos);
            float playerDistance = Vector3.Distance(playerPos, nextPos);

            partialProgressToNext = Mathf.Clamp01(1f - (playerDistance / totalDistance));
        }
        else
        {
            partialProgressToNext = 0f; 
        }
    }

    public void CheckpointPassed(int checkpointID)
    {
        if (checkpointID == currentCheckpointIndex)
        {
            currentCheckpointIndex++;

            if (currentCheckpointIndex >= totalCheckpoints)
            {
                foreach(GameObject powerUp in PowerUpManager.list)
                {
                    if (powerUp != null)
                    {
                        powerUp.SetActive(true);
                    }
                }
                currentCheckpointIndex = 0;
                lapCount++;
                lapText.text = lapCount.ToString();
                Debug.Log("Lap Completed! Total Laps: " + lapCount);
            }
        }
        else
        {
            Debug.Log("Wrong checkpoint!");
        }
    }
}
