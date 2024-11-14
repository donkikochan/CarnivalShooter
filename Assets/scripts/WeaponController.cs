using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public void EnableWeapon()
    {
        gameObject.SetActive(true);
    }

    public void DisableWeapon()
    {
        gameObject.SetActive(false);
    }
}
