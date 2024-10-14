using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    public GameObject projectile;
    protected Transform firePoint, weaponOrigin;

    public void Equip()
    {
        firePoint = GetComponentInChildren<Transform>();
        PlayerController.playerFireEvent += FireWeapon;

    }

    public void UnEquip()
    {
        PlayerController.playerFireEvent -= FireWeapon;
        Destroy(this.gameObject);
    }

    public virtual void FireWeapon()
    {

    }

}
