using System.Collections;
using System.Collections.Generic;
using Autohand.Demo;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private XRControllerEvent controller;
    private GameObject robotHand;
    
    void Start()
    {
        controller = GetComponent<XRControllerEvent>();
        robotHand = GameObject.Find("RobotHand (R)");
        
        if (robotHand != null)
        {
            Debug.Log("Objeto encontrado: " + robotHand.name);
        }
        else
        {
            Debug.LogWarning("El objeto 'RobotHand (R)' no fue encontrado.");
        }
        
        if (!controller.link)
        {
            controller.link = robotHand.GetComponent<XRHandControllerLink>();
        }
    }
    
    public void EnableWeapon()
    {
        gameObject.SetActive(true);
    }

    public void DisableWeapon()
    {
        gameObject.SetActive(false);
    }
}
