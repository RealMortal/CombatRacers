using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotOneKill : PowerUp
{

    public bool isAbleToOneShot;
    private GameObject activatingPlayer;

    public override void ActivatePowerUp(GameObject player)
    {
        PlayerPowerUpManager manager = player.GetComponentInParent<PlayerPowerUpManager>();
        if (manager != null)
        {
            Debug.Log("Teleport Power-Up Stored");
            activatingPlayer = player.transform.root.gameObject; 
            manager.StorePowerUp(this, StartOneShot);
        }
    }
    
    private void StartOneShot()
    {
        StartCoroutine(StartTimer());

    }
    private IEnumerator StartTimer()
    {
        var controller = activatingPlayer.GetComponent<NewCarController>();

        if (controller != null)
        {
            controller.isAbleToOneShot = true;
            yield return new WaitForSeconds(1.5f);
            controller.isAbleToOneShot = false;
        }
    }

    private void OnDisable()
    {
        isAbleToOneShot = false;
    }

}
