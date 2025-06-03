using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStartCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;    // UI text element to display countdown numbers
    public float countdownDuration = 3f;     // Duration of countdown in seconds

    private GameObject[] playersToFreeze;    // Array holding references to player GameObjects to freeze/unfreeze

    // Assign the two player GameObjects to freeze during countdown
    public void SetPlayersToFreeze(GameObject P1, GameObject P2)
    {
        playersToFreeze = new GameObject[2];
        playersToFreeze[0] = P1;
        playersToFreeze[1] = P2;
    }

    // Start the countdown coroutine that freezes players and counts down
    public void FreezePlayers()
    {
        StartCoroutine(CountdownAndStart());
    }

    // Coroutine to handle countdown timer and enabling player controls after countdown
    IEnumerator CountdownAndStart()
    {
        // Disable player inputs so they cannot move during countdown
        foreach (var player in playersToFreeze)
        {
            player.GetComponent<PlayerInput>().DeactivateInput();
        }

        float timeLeft = countdownDuration;

        // Countdown loop that updates the UI every second
        while (timeLeft > 0)
        {
            countdownText.text = Mathf.Ceil(timeLeft).ToString();  // Show remaining time rounded up
            yield return new WaitForSeconds(1f);                   // Wait for 1 second
            timeLeft--;
        }

        // Show "GO!" message after countdown ends
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        // Hide countdown UI
        gameObject.SetActive(false);

        // Reactivate player inputs to start the game
        foreach (var player in playersToFreeze)
        {
            player.GetComponent<PlayerInput>().ActivateInput();
        }
    }
}
