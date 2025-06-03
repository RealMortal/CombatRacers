using UnityEngine;

public class CheckPoint : MonoBehaviour
{
     public int checkpointID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<LapManager>().CheckpointPassed(checkpointID);
        }
    }
}
