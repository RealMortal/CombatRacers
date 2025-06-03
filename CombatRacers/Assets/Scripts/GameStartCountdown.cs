using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStartCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 3f;

    private GameObject[] playersToFreeze;

    
    public void SetPlayersToFreeze(GameObject P1, GameObject P2)
    {
        playersToFreeze = new GameObject[2];
        playersToFreeze[0] = P1;
        playersToFreeze[1] = P2;
    }

    public void FreezePlayers()
    {
        StartCoroutine(CountdownAndStart());

    }
    IEnumerator CountdownAndStart()
    {
        foreach (var player in playersToFreeze)
        {
            player.GetComponent<PlayerInput>().DeactivateInput();
        }

        float timeLeft = countdownDuration;
        while (timeLeft > 0)
        {
            countdownText.text = Mathf.Ceil(timeLeft).ToString();
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        foreach (var player in playersToFreeze)
        {
            player.GetComponent<PlayerInput>().ActivateInput();
        }
    }
}
