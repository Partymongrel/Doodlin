using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Gun : Weapon
{

    public float bulletForce = 20f;

    public override void FireWeapon()
    {
        Rigidbody2D bulletRB = Instantiate(projectile, firePoint.position, firePoint.rotation).GetComponent<Rigidbody2D>();
        bulletRB.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

    }

}
