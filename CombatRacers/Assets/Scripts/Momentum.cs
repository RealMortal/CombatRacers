using System.Collections;
using Unity.Cinemachine;
using UnityEngine;


public class Momentum : MonoBehaviour
{
    public float rayDistance = 20f;
    public float boostMultiplier = 1.2f;
    public float boostDuration = 1f;

    private Rigidbody rb;
    private NewCarController controller;
    private float originalAcceleration;
    private Coroutine activeBoost;
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
    }
    public void SetPowerUp(NewCarController controller)
    {
        this.controller = controller;
        originalAcceleration = controller.GetAcceleration();

    }
    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                float mass = hit.transform.gameObject.GetComponent<Rigidbody>().mass;
                
                float proximity = (mass > rb.mass) ? (rayDistance - hit.distance) / rayDistance : 0;

                if (proximity > 0.3f) 
                {
                    TriggerBoost();
                }
            }
        }
    }

    void TriggerBoost()
    {
        if (activeBoost != null)
        {
            StopCoroutine(activeBoost);
            controller.SetAcceleration(originalAcceleration); 
        }

        activeBoost = StartCoroutine(ApplyBoost());
    }

    IEnumerator ApplyBoost()
    {

        controller.SetAcceleration(originalAcceleration * boostMultiplier);
       

        yield return new WaitForSeconds(boostDuration);

        controller.SetAcceleration(originalAcceleration);
     

        activeBoost = null;
    }

  
}
